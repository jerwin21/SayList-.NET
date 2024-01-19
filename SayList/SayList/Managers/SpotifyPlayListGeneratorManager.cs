using System.Text;
using Microsoft.Extensions.Caching.Memory;
using SayList.Interfaces;
using SayList.Things;
using SayList.Utilities;

namespace SayList.Managers;

public class SpotifyPlayListGeneratorManager : IPlaylistGeneratorManager
{
    IAuthorizationEngine _authorizationEngine;
    IPlaylistGeneratorEngine _playlistGeneratorEngine;

    private const string _TrackQueryType = "track";
    private const int _TrackQueryLimit = 50;

    private IMemoryCache _authTokenCache;

    #region Constructor
    public SpotifyPlayListGeneratorManager(IAuthorizationEngine authorizationEngine, IMemoryCache authTokenCache, IPlaylistGeneratorEngine playlistGeneratorEngine)
    {
        _authorizationEngine = authorizationEngine;
        _playlistGeneratorEngine = playlistGeneratorEngine;
        _authTokenCache = authTokenCache;
    }

    #endregion

    #region Helpers

    private static string RemoveSomePunctuation(string s)
    {
        // Define the punctuation to remove
        string[] punctuationToRemove = new string[] { ".", ",", "?", "!" };

        foreach (var punctuation in punctuationToRemove)
        {
            s = s.Replace(punctuation, string.Empty);
        }

        return s;
    }

    public static string RemoveAllPunctuation(string phrase)
    {
        // Create a StringBuilder for efficient string manipulation
        StringBuilder cleanPhrase = new StringBuilder();

        foreach (char c in phrase)
        {
            // Check if the character is neither a punctuation mark nor a space
            if (!char.IsPunctuation(c) && c != ' ')
            {
                cleanPhrase.Append(c);
            }
        }

        return cleanPhrase.ToString();
    }

    private static List<string> SplitWords(string message)
    {
        message = RemoveSomePunctuation(message);

        List<string> words = new List<string>();

        // Split by spaces
        string[] splitBySpaces = message.Split(' ');

        foreach (var item in splitBySpaces)
        {
            // Split further by hyphens if present and add to words list
            words.AddRange(item.Split('-'));
        }

        return words;
    }

    private static string MakePhrase(List<string> words, int startIndex, int wordCount)
    {
        // Ensure the range is within the bounds of the list
        int safeEndIndex = Math.Min(startIndex + wordCount, words.Count);
        
        // Join the specified range of words into a single string
        return string.Join(" ", words.GetRange(startIndex, safeEndIndex - startIndex)) + " ";
    }

    private async Task<SpotifyAccessTokenResponse> GetToken()
    {
        return await _authorizationEngine.GetAccessTokenAsync();
    }

    private async Task<SpotifyTrackSearchResponse> SearchTracks(SpotifyAccessTokenResponse token, string phrase)
    {
        return await _playlistGeneratorEngine.SearchSpotify(token,
            new SpotifySearchRequest
            {
                Limit = _TrackQueryLimit,
                Query = phrase,
                Type = _TrackQueryType,
            });
    }

    private List<SpotifyTrack> RankedMatches(SpotifyTrackSearchResponse searchResponse, string phrase)
    {
        string[] punctuationToRemove = new string[] { ".", ",", "?", "!", " " };

        List<SpotifyTrack> matches = searchResponse.Items
            .Where(i => RemoveAllPunctuation(i.Name).ToLower().Equals(RemoveAllPunctuation(phrase).ToLower()))
            .OrderByDescending(i => i.Popularity).ToList();

        return matches;
    }

    #endregion

    #region IPlaylistGeneratorManager
    
    public async Task<SaylistBuildResult> BuildPlaylist(string userMessage)
    {
        List<string> words = SplitWords(userMessage);

        SayListState state = new SayListState
        {
            BlockIndex = 0,
            WordsRemaining = words.Count,
            WordIndex = 0,
            SetLength = 0,
            Playlist = new List<SayListBlock>(),
        };

        SayListBlock currentBlock = null;

        bool forward = true;
        bool complete = true;

        bool wordFailed = false;

        try
        {
            while (state.WordsRemaining > 0)
            {
                //if some block can't find a phrase, and we end up back at beginning, pop it, and start again there. 
                if (state.BlockIndex == -1)
                {
                    var failedBlock = state.Playlist.Pop();
                    state.BlockIndex = failedBlock.Index;
                    state.WordIndex = failedBlock.WordIndex + 1;
                    state.WordsRemaining--;
                    wordFailed = true;

                    currentBlock = new SayListBlock(state.BlockIndex, state.WordIndex, Math.Min(state.WordsRemaining, 7));
                    state.Playlist.Add(currentBlock);
                    //if troublesome word is the last word, we should not add a new block to the playlsit and just exit the loop
                    if (state.WordsRemaining <= 0)
                        break;

                }
                else if (state.BlockIndex < state.Playlist.Count)
                {
                    currentBlock = state.Playlist[state.BlockIndex];
                }
                else
                {
                    //TODO: make max size configurable somehow
                    currentBlock = new SayListBlock(state.BlockIndex, state.WordIndex, Math.Min(state.WordsRemaining, 7));

                    state.Playlist.Add(currentBlock);
                }

                if (currentBlock.SizesNotTried.Any())
                {
                    int size = currentBlock.SizesNotTried.Pop();

                    if (size <= state.WordsRemaining)
                    {
                        currentBlock.SizeLastTried = size;
                        currentBlock.Phrase = MakePhrase(words, state.WordIndex, size);

                        SpotifyAccessTokenResponse token = await GetToken();

                        SpotifyTrackSearchResponse response = await SearchTracks(token, currentBlock.Phrase);

                        List<SpotifyTrack> matches = RankedMatches(response, currentBlock.Phrase);

                        if (matches.Any())
                        {
                            currentBlock.Track = new SayListTrack { Track = matches.First() };
                            state.WordIndex += size;
                            state.WordsRemaining -= size;
                            state.BlockIndex++;
                            state.SetLength++;
                        }
                    }
                }
                else
                {
                    state.BlockIndex--;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new SaylistBuildResult
            {
                IsSuccess = false,
                IsComplete = complete,
                Message = $"Unable to build playlist do to unknown exception: {e}",
            };
        }


        return new SaylistBuildResult
        {
            IsSuccess = true,
            IsComplete = complete,
            Message = "Enjoy the playlist!",
            Tracks = state.Playlist.Select(t => t.Track.Track).ToList()
        };
    }

    public Task<SpotifyPlaylist> CreatePlaylist(IEnumerable<SpotifyTrack> tracks)
    {
        throw new NotImplementedException();
    }

    #endregion

}
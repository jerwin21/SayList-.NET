import requests
import random
import string

class PlaylistBuildingBlock:
    def __init__(self, index, word_index, not_tried, last_tried, track, phrase):
        self.index = index
        self.word_index = word_index
        self.not_tried = not_tried
        self.last_tried = last_tried
        self.track = track
        self.phrase = phrase


def get_access_token():
    url = "https://accounts.spotify.com/api/token"
    headers = {
        "Content-Type": "application/x-www-form-urlencoded"
    }

    data = {
        "grant_type": "client_credentials",
        "client_id": "a1143666ce6a418ab007c2501431c096",
        "client_secret": "e6138706c3d64583ad685dcfd185d8c8"
    }
    
    response = requests.post(url, headers=headers, data=data)
    
    if response.status_code == 200:
        # Request successful, handle the response data (access token)
        access_token = response.json().get("access_token")
        print(f"Access Token: {access_token}")
        return access_token
    else:
        # Request failed, print error message
        print(f"Error: {response.status_code} - {response.text}")
        return None
    
def search_spotify_tracks(query, access_token, result_limit):
    url = "https://api.spotify.com/v1/search"
    
    headers = {
        "Authorization": f"Bearer {access_token}"
    }

    params = {
        "q": query,
        "type": "track",
        "limit": result_limit,
    }

    response = requests.get(url, headers=headers, params=params)

    if response.status_code == 200:
        # Request successful, handle the response data (list of tracks)
        tracks = response.json().get("tracks", {}).get("items", [])
        return tracks
    else:
        # Request failed, print error message
        print(f"Error: {response.status_code} - {response.text}")
        return None
    
#takes list of tracks, and finds ones that match the phrase
#def find_matches(tracks, words):
    
    
    

#splits words into array and removes punctuation        
def split_words(message):
    # Define a translation table to remove punctuation
    punctuation_to_remove = ".,?!"
    translation_table = str.maketrans("", "", punctuation_to_remove)

    words = []
    for word in message.translate(translation_table).split():
        # Split further by hyphens if present
        words.extend(word.split("-"))

    return words

def make_phrase(words, start_index, word_count):
    phrase = " ".join(words[start_index:start_index + word_count])
    return phrase

def generate_phrase_size_list(max_size):
    random_list = random.sample(range(1, max_size + 1), max_size)
    return random_list

def remove_all_punctuation(phrase):
    translation_table = str.maketrans("", "", string.punctuation)
    clean_phrase = phrase.translate(translation_table)
    return clean_phrase.replace(" ", "")

def get_matches(phrase, tracks):
    matches = []

    for track in tracks:
        #match if track name matches phrase, with or without spaces and punctuation
        if remove_all_punctuation(track["name"]).lower() == remove_all_punctuation(phrase).lower():
            matches.append(track)
    return matches

def rank_matches_popularity(matches):
    ranked_matches = sorted(matches, key=lambda x: x["popularity"], reverse=True)
    return ranked_matches

def build_playlist(words):
    playlist = []
    words_left = len(words) 
    word_index = 0
    block_index = 0
    set_length = 0
    
    while words_left > 0:
        #build block
        print(f"DEBUG: block index: {block_index}, playlist length: {len(playlist)}")
        #if block already exists (back tracking), just grab it
        if block_index < set_length:
            print(f"block at {block_index} exists, continuing with it")
            block = playlist[block_index]
        else: #if block doesn't exist yet, build it
            print(f"block at {block_index} DNE, making a new one")
            if words_left < 6:
                size_list = generate_phrase_size_list(words_left)
            else:
                size_list = generate_phrase_size_list(6)
            block = PlaylistBuildingBlock(block_index, word_index, size_list, None, None, None)
            playlist.append(block)
        #if block has sizes not tried yet, continue with that block. If not, backtrack.
        print(f"DEBUG: not tried list for block:{block_index} is: {block.not_tried}")
        if block.not_tried:
            #get phrase and get songs
            size = block.not_tried.pop()
            print(f"DEBUG: attempting size: {size} with words left: {words_left}")
            if size <= words_left:
                block.last_tried = size
                block.phrase = make_phrase(words, word_index, size)
                access_token = get_access_token()
                print(f"DEBUG: searching for matches with {size} words, and phrase: {block.phrase}")
                tracks = search_spotify_tracks(block.phrase, access_token, 50)
                #get matches
                ranked_matches = rank_matches_popularity(get_matches(block.phrase, tracks))
                #if there are no ranked matches, we need to end up trying another size from the not tried list. Does that mean just exit this loop without adding to playlist
                # if(block.phrase.lower() == 'hello'):
                #     for track in tracks:
                #         print(f"Tracks returned: {track['name']}")
                #     print(f"Matches: {ranked_matches[0]['name']}")
                #if there is/are ranked match(es), add to playlist, and increment the word_index and block_index accordingly
                if ranked_matches:
                    block.track = ranked_matches[0]
                    print(f"found some matches! Adding track {block.track['name']} to block:{block_index} and moving on")
                    word_index += size
                    words_left -= size
                    block_index += 1
                else: 
                    print(f"DEBUG: not matches somehow")
            #don't change the block_index or nothing, then when we are back at top of loop, we grab the same block from the playlist
            else:
                print(f"DEBUG: block {block_index} has no more sizes to try")
        #if no more in the not tried list, pop it from the playlist, return to the previous block (decrement block index), decrement word index so we can get the right phrase and         
        else:
            print(f"No more in tried list for block:{block_index}... backtracking... ")
            block_index -= 1
            word_index -= playlist[block_index].last_tried
            #playlist.pop()                  --- gonna try not popping from the playlist here, 

    print("\n\n")
    print("RESULTS")
    for song in playlist:
        print(song.track["name"] + ", " + song.track["artists"][0]["name"])       
            
    

def main():
    
    message = input("Enter a message you'd like to turn into a playlist:  ")
    
    access_token = get_access_token()
    
    words = split_words(message)
    
    #build track list
    build_playlist(words)


if __name__ == "__main__":
    main()
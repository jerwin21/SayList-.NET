**Saylist**

**Afraid to say it yourself, use a Saylist**

This was originally inteded to serve as the middle man between a web app that I was going to write using .NET and the spotify developer API.
I did not spend enough time reading the Spotify Developer API documentation, and ended up with most of the API written after realizing that it really wasn't an ideal design,
and things would be better off all in the client. But it was a fun exercise, and it has allowed me to put some c# on my github. 

There's a fun little algorithm in here that utilizes the spotify API and a sort of depth first search, that when given a message from the user, attempts to return a list of songs from spotify whose titles
read out the users message. It could for sure be improved.

I didn't quite get to the part where it automatically creates the playlist for you. This is where the web app piece needs to come in, because it needs to ask users for certains permissions (OAuth scopes) 
to get a special access token to create playlists. 

**If you want to run this your self**

you will need to register your own app and get your own client_id and client_secret from Spotify. Can be done here https://developer.spotify.com/dashboard
I utilized .NET user secrets to hide my id and secret, but you could use appsettings.json

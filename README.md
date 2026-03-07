# Episodes

Tv Show Tracker

Features (v1)

• Search for a TV show
• Display show metadata (name, year, description, poster, seasons).
• User can add shows to their “Watchlist.”
• Track watched episodes for each show.
○ watched and unwatched
• Simple user authentication (email/password)
○ multi-users
• Synch show up
○ manual
○ background process

Optional Features (future versions)

• New Episode notifications
• TV recommendations
• up next episodes for each show

Endpoints
📌
AUTHENTICATION

Register a new user
POST /api/auth/register

Log in
POST /api/auth/login

Refresh access token
POST /api/auth/refresh

Current authenticated user
GET /api/me

🎬
SHOWS (Read-Only TMDB Catalog)

Query shows (paginated)
GET /v1/shows

Get a single show (with seasons + optional summaries)
GET /shows/{showId}

Get episodes for a specific season
GET /shows/{showId}/seasons/{seasonNumber}/episodes

⭐
WATCHLIST (Add/Remove Shows a User Follows)

Get a user’s watchlist
GET /users/{userId}/watchlist

Add a show to the watchlist
POST /users/{userId}/watchlist/{showId}

Remove a show from the watchlist
DELETE /users/{userId}/watchlist/{showId}

✔️
EPISODE WATCH STATUS (Mark Episodes Watched/Unwatched)

Mark an episode as watched
POST /users/{userId}/shows/{showId}/episodes/{episodeId}/watched

Unmark an episode as watched
DELETE /users/{userId}/shows/{showId}/episodes/{episodeId}/watched

❤️
HEALTH CHECKS

API health check
GET /health

Database connectivity check
GET /health/verify

👤
USER PROFILE

Get user profile
GET /api/v1/users/{userId}

Update user profile
PUT /api/v1/users/{userId}

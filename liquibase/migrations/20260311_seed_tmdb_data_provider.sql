INSERT INTO episodes.tv_data_providers (id, name)
VALUES (1, 'tmdb')
ON CONFLICT (name) DO NOTHING;

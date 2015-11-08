CREATE VIEW canc_metadata_tbl AS
SELECT P.usr_name, V.id3v2_title, V.id3v2_author, V.id3v2_album, V.id3v2_year, V.id3v2_genre, V.id3v2_lyrics, V.submission_date, C.song_directory, C.local_song_id
FROM canciones_tbl AS C 
	JOIN propiedades_tbl AS P ON C.local_song_id = P.local_song_id 
	JOIN versiones_tbl AS V ON C.metadata_id = V.local_version_id
CREATE TABLE propiedades_tbl
(
	usr_name VARCHAR(32) NOT NULL,
	local_song_id INT NOT NULL,
	song_name VARCHAR(32) NOT NULL,
	CONSTRAINT pk_propiedad_id PRIMARY KEY(usr_name, local_song_id)
)

CREATE TABLE canciones_tbl
(
	local_song_id INT IDENTITY,
	cloud_song_id INT DEFAULT -1,
	metadata_id INT DEFAULT -1,
	song_directory VARCHAR(100),
	CONSTRAINT pk_cancion_id PRIMARY KEY(local_song_id)
)

CREATE TABLE versiones_tbl
(
	local_version_id INT IDENTITY,
	local_song_id INT NOT NULL, 
	cloud_version_id INT DEFAULT -1,
	cloud_song_id INT DEFAULT -1,
	submission_date DATETIME DEFAULT GETDATE(),
	id3v2_title VARCHAR(50),
	id3v2_author VARCHAR(50),
	id3v2_lyrics VARCHAR(50),
	id3v2_album VARCHAR(50),
	id3v2_genre VARCHAR(50),
	id3v2_year INT,
	CONSTRAINT pk_version_id PRIMARY KEY(local_version_id, local_song_id, submission_date)
)


ALTER TABLE propiedades_tbl
ADD CONSTRAINT fk_song_id
FOREIGN KEY (local_song_id)
REFERENCES canciones_tbl(local_song_id)


ALTER TABLE versiones_tbl
ADD CONSTRAINT fk_canc_song_id
FOREIGN KEY (local_song_id)
REFERENCES canciones_tbl(local_song_id)
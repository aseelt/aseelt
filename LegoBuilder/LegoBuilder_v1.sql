USE master;
GO

DROP DATABASE IF EXISTS LegoBuilder;

CREATE DATABASE LegoBuilder;
GO

USE LegoBuilder;
GO

BEGIN TRANSACTION

CREATE TABLE colours
(
	colours_id INT IDENTITY (1,1),
	id INT NOT NULL,
	name VARCHAR(200) NOT NULL,
	rgb CHAR(6) NOT NULL,
	is_trans BIT NOT NULL, 
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_colours PRIMARY KEY (colours_id),
	CONSTRAINT UQ_id_c UNIQUE(id)
)

CREATE TABLE part_categories
(
	part_categories_id INT IDENTITY (1,1),
	id INT NOT NULL,
	name VARCHAR(200) NOT NULL,
	part_count INT NOT NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_part_categories PRIMARY KEY (part_categories_id),
	CONSTRAINT UQ_id_pc UNIQUE(id)
)

CREATE TABLE themes
(
	themes_id INT IDENTITY (1,1),
	id INT NOT NULL,
	parent_id INT NULL,
	name VARCHAR(200) NOT NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_themes PRIMARY KEY (themes_id),
	CONSTRAINT UQ_id_t UNIQUE(id)
)

CREATE TABLE users
(
	user_id INT IDENTITY (7,7),
	username VARCHAR(50) NOT NULL,
	password_hash VARCHAR(200) NOT NULL,
	salt VARCHAR(200) NOT NULL,
	first_name VARCHAR(50) NOT NULL,
	last_name VARCHAR(50) NOT NULL,
	email VARCHAR(200) NOT NULL,
	role VARCHAR(20) NOT NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_user PRIMARY KEY (user_id),
	CONSTRAINT UQ_username UNIQUE(username), 
	CONSTRAINT CK_role CHECK (role = 'user' OR role = 'overlord')
)

CREATE TABLE parts
(
	parts_id INT IDENTITY (1,1),
	part_num VARCHAR(50) NOT NULL,
	name VARCHAR(500) NOT NULL,
	part_cat_id INT NOT NULL,
	year_from INT NULL,
	year_to INT NULL,
	part_url VARCHAR(500) NULL,
	part_img_url VARCHAR(500) NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_parts PRIMARY KEY (parts_id),
	CONSTRAINT UQ_part_num_p UNIQUE(part_num),
	CONSTRAINT FK_parts_part_categories FOREIGN KEY (part_cat_id) REFERENCES part_categories(id)
)

CREATE TABLE sets
(
	sets_id INT IDENTITY (1,1),
	set_num VARCHAR(50) NOT NULL,
	name VARCHAR(500) NOT NULL,
	year INT NOT NULL,
	theme_id INT NOT NULL,
	num_parts INT NOT NULL,
	set_img_url VARCHAR(500) NULL,
	last_modified_dt DATETIME NOT NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_sets PRIMARY KEY (sets_id),
	CONSTRAINT UQ_set_num_s UNIQUE(set_num),
	CONSTRAINT FK_sets_themes FOREIGN KEY (theme_id) REFERENCES themes(id)
)

CREATE TABLE instructions
(
	instructions_id INT IDENTITY(1,1),
	set_id VARCHAR(50) NOT NULL,
	url VARCHAR(500) NULL, 
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_instructions PRIMARY KEY (instructions_id),
	CONSTRAINT FK_instructions_set FOREIGN KEY (set_id) REFERENCES sets(set_num)
)

CREATE TABLE sets_parts
(
	sets_parts_id INT IDENTITY (1,1),
	set_num VARCHAR(50) NOT NULL,
	id INT NOT NULL,
	inv_part_id INT NULL,
	part_num VARCHAR(50) NOT NULL,
	colour_id INT NOT NULL,
	quantity INT NOT NULL,
	is_spare BIT NOT NULL,
	element_id VARCHAR(20) NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_sets_parts PRIMARY KEY (sets_parts_id),
	CONSTRAINT FK_sets_parts_sets FOREIGN KEY (set_num) REFERENCES sets(set_num),
	CONSTRAINT FK_sets_parts_parts FOREIGN KEY (part_num) REFERENCES parts(part_num),
	CONSTRAINT FK_sets_parts_colours FOREIGN KEY (colour_id) REFERENCES colours(id)
)

CREATE TABLE users_sets
(
	users_sets_id INT IDENTITY(1,1),
	user_id INT NOT NULL,
	set_num VARCHAR(50) NOT NULL,
	quantity INT DEFAULT 1 NOT NULL,
	is_favourite BIT DEFAULT 0 NOT NULL,
	is_active BIT DEFAULT 1 NOT NULL,
	lb_creation_date DATETIME DEFAULT getdate() NOT NULL,
	lb_update_date DATETIME DEFAULT getdate() NOT NULL,

	CONSTRAINT PK_users_sets PRIMARY KEY (users_sets_id),
	CONSTRAINT FK_users_sets_users FOREIGN KEY (user_id) REFERENCES users(user_id),
	CONSTRAINT FK_users_sets_sets FOREIGN KEY (set_num) REFERENCES sets(set_num),
	CONSTRAINT CK_quantity CHECK (quantity > -1)
)
COMMIT
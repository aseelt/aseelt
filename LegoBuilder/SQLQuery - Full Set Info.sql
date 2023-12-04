-- for set information
SELECT sets_parts_id, sets_parts.set_num, sets_parts.is_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, 
sets.name AS set_name, sets.year AS year_released, sets.num_parts AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified,
url AS instructions_url,
themes.name AS set_theme, 
sets_parts.part_num, element_id, sets_parts.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url,
colours.name AS colour, rgb, is_trans
FROM sets_parts
JOIN sets ON sets_parts.set_num = sets.set_num
LEFT JOIN instructions ON sets.set_num = instructions.set_id
JOIN themes ON sets.theme_id = themes.id
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id
JOIN colours ON sets_parts.colour_id = colours.id
WHERE sets_parts.set_num = '10297-1'
ORDER BY sets_parts_id;

SELECT *
FROM sets_parts
JOIN sets ON sets_parts.set_num = sets.set_num
LEFT JOIN instructions ON sets.set_num = instructions.set_id
JOIN themes ON sets.theme_id = themes.id
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id
JOIN colours ON sets_parts.colour_id = colours.id
WHERE sets_parts.set_num = '10297-1'
ORDER BY sets_parts_id;

-- for full users sets and parts information
SELECT username, users.is_active AS user_active,
sets_parts.set_num, is_favourite, sets_parts.is_active AS set_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, 
sets.name AS set_name, users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts * users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified,
url AS instructions_url,
themes.name AS set_theme, 
sets_parts.part_num, element_id, sets_parts.quantity * users_sets.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url,
colours.name AS colour, rgb, is_trans
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num
JOIN sets ON sets_parts.set_num = sets.set_num
LEFT JOIN instructions ON sets.set_num = instructions.set_id
JOIN themes ON sets.theme_id = themes.id
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id
JOIN colours ON sets_parts.colour_id = colours.id
WHERE users.username = 'aseelt'
ORDER BY sets_parts.set_num, sets_parts.part_num; 

-- for users sets information
SELECT DISTINCT username, users.is_active AS user_active,
sets.set_num, is_favourite, sets.is_active AS set_active, sets.lb_creation_date, sets.lb_update_date, sets.name AS set_name, 
users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts * users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified,
url AS instructions_url,
themes.name AS set_theme  
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
JOIN sets ON users_sets.set_num = sets.set_num
LEFT JOIN instructions ON sets.set_num = instructions.set_id
JOIN themes ON sets.theme_id = themes.id 
WHERE users.username = 'aseelt' 
ORDER BY sets.set_num; 

-- for users themes information
SELECT DISTINCT username, users.is_active AS user_active,
SUM(users_sets.quantity) AS set_quantity, SUM(sets.num_parts * users_sets.quantity) AS total_num_parts, 
themes.name AS set_theme  
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
JOIN sets ON users_sets.set_num = sets.set_num
LEFT JOIN instructions ON sets.set_num = instructions.set_id
JOIN themes ON sets.theme_id = themes.id 
WHERE users.username = 'aseelt' 
GROUP BY username, users.is_active, themes.name
ORDER BY themes.name; 

-- for users parts and colour information 
SELECT DISTINCT username, users.is_active AS user_active, 
sets_parts.part_num, element_id, SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url,
colours.name AS colour, rgb, is_trans
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num 
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id
JOIN colours ON sets_parts.colour_id = colours.id
WHERE users.username = 'aseelt'
GROUP BY username, users.is_active, sets_parts.part_num, element_id, parts.name, part_categories.name, part_url, part_img_url,colours.name, rgb, is_trans
ORDER BY element_id; 

-- for users parts only information 
SELECT DISTINCT username, users.is_active AS user_active, 
sets_parts.part_num, SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url 
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num 
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id 
WHERE users.username = 'aseelt'
GROUP BY username, users.is_active, sets_parts.part_num, parts.name, part_categories.name, part_url, part_img_url 
ORDER BY sets_parts.part_num; 

-- for users colour information only
SELECT username, users.is_active AS user_active, 
SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, 
colours.name AS colour, rgb, is_trans
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num 
JOIN parts ON sets_parts.part_num = parts.part_num 
JOIN colours ON sets_parts.colour_id = colours.id
WHERE users.username = 'aseelt'
GROUP BY username, users.is_active, colours.name, rgb, is_trans
ORDER BY colours.name; 

-- for users parts categories information 
SELECT DISTINCT username, users.is_active AS user_active, 
SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, part_categories.name AS part_cat_name
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num 
JOIN parts ON sets_parts.part_num = parts.part_num
JOIN part_categories ON parts.part_cat_id = part_categories.id
WHERE users.username = 'aseelt'
GROUP BY username, users.is_active, part_categories.name 
ORDER BY part_categories.name; 

-- for users total parts information 
SELECT DISTINCT username, users.is_active AS user_active, 
SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity
FROM users
LEFT JOIN users_sets ON users.user_id = users_sets.user_id
LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num 
JOIN parts ON sets_parts.part_num = parts.part_num 
WHERE users.username = 'aseelt'
GROUP BY username, users.is_active 
ORDER BY username; 
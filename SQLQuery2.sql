DELETE FROM Products WHERE Id = 8

SELECT Id, Name FROM Products ORDER BY Id

DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] LIKE '%AddNamGender%'
DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] LIKE '%FixNamGender%'
DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] LIKE '%RemoveDuplicate%'

SELECT * FROM [__EFMigrationsHistory]

DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260321081119_UpdateGenderData'

DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260321080554_AddGenderToProduct'

DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260321080554_AddGenderToProduct'

SELECT * FROM Categories

DELETE FROM Products

DELETE FROM Categories

SELECT * FROM [__EFMigrationsHistory]

DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260321081119_UpdateGenderData'

INSERT INTO Categories ( Name) VALUES (N'Vòng - Lắc')
INSERT INTO Categories ( Name) VALUES (N'Nhẫn')
INSERT INTO Categories ( Name) VALUES (N'Dây Chuyền')
INSERT INTO Categories ( Name) VALUES (N'Bông Tai')
INSERT INTO Categories ( Name) VALUES (N'Trang Sức Đôi')
INSERT INTO Categories ( Name) VALUES (N'Trang Sức Bộ')
INSERT INTO Categories ( Name) VALUES (N'Phụ Kiện')

SELECT * FROM Products
SELECT * FROM Categories

DELETE FROM Categories;

DBCC CHECKIDENT ('Categories', RESEED, 0);

INSERT INTO Categories (Name) VALUES (N'Vòng - Lắc')
INSERT INTO Categories (Name) VALUES (N'Nhẫn')
INSERT INTO Categories (Name) VALUES (N'Dây Chuyền')
INSERT INTO Categories (Name) VALUES (N'Bông Tai')
INSERT INTO Categories (Name) VALUES (N'Trang Sức Đôi')
INSERT INTO Categories (Name) VALUES (N'Trang Sức Bộ')
INSERT INTO Categories (Name) VALUES (N'Phụ Kiện')

UPDATE Products SET CategoryId = 1 WHERE CategoryId = 8
UPDATE Products SET CategoryId = 2 WHERE CategoryId = 9
UPDATE Products SET CategoryId = 3 WHERE CategoryId = 10
UPDATE Products SET CategoryId = 4 WHERE CategoryId = 11
UPDATE Products SET CategoryId = 5 WHERE CategoryId = 12
UPDATE Products SET CategoryId = 6 WHERE CategoryId = 13
UPDATE Products SET CategoryId = 7 WHERE CategoryId = 14

SELECT * FROM Categories
SELECT * FROM Products

-- Xem trước để chắc chắn đúng sản phẩm
SELECT * FROM Products WHERE Name LIKE N'%Kim Bảo Như Ý%'
DELETE FROM Products WHERE Id = 35
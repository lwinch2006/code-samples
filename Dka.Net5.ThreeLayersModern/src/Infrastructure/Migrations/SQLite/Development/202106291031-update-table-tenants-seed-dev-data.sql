BEGIN TRANSACTION;

INSERT OR IGNORE INTO [Tenants] ([Id], [Name])
VALUES
    ('41ec4785-859f-429e-a399-abd86f4f9f40', 'Umbrella Corporation'),
    ('0aefe699-ac98-4ebb-9588-3e9ebcffe3aa', 'Cyberdyne Systems'),
    ('4d4db551-bb4c-4a06-b300-bcd6a3307219', 'OCP');

COMMIT;
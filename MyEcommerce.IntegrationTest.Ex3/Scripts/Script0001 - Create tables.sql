use MyEcommerce;
go

if object_id(N'dbo.customers', N'U') is null
    create table customers
    (
        id int identity primary key,
        name varchar(300),
        birth DATETIME,
        email VARCHAR(100)
    )
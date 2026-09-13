/*
 Event & Parking Reservation System
 SQL Server starter schema based on BRD v1.3 + Phase 02.
 The API also uses EF Core EnsureCreated for quick student-project setup.
*/

IF DB_ID('EventParkingReservationSystemDb') IS NULL
    CREATE DATABASE EventParkingReservationSystemDb;
GO

USE EventParkingReservationSystemDb;
GO

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Status INT NOT NULL DEFAULT 1, -- 1 Active, 2 Deactivated
    EmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationTokenHash NVARCHAR(128) NULL,
    EmailVerificationTokenExpiresAtUtc DATETIME2 NULL,
    PasswordResetTokenHash NVARCHAR(128) NULL,
    PasswordResetTokenExpiresAtUtc DATETIME2 NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL
);
GO

CREATE TABLE AdminUsers (
    AdminUserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Venues (
    VenueId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Address NVARCHAR(300) NOT NULL,
    TotalCapacity INT NOT NULL CHECK (TotalCapacity > 0)
);
GO

CREATE TABLE EventCategories (
    EventCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    VenueId INT NOT NULL,
    EventCategoryId INT NOT NULL,
    EventDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    TicketPrice DECIMAL(10,2) NOT NULL CHECK (TicketPrice >= 0),
    Capacity INT NOT NULL CHECK (Capacity > 0),
    ParkingFee DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (ParkingFee >= 0),
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES Venues(VenueId),
    CONSTRAINT FK_Events_Categories FOREIGN KEY (EventCategoryId) REFERENCES EventCategories(EventCategoryId),
    CONSTRAINT CK_Events_Time CHECK (StartTime < EndTime)
);
GO

CREATE TABLE Seats (
    SeatId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    SeatNumber NVARCHAR(20) NOT NULL,
    SeatType NVARCHAR(50) NULL,
    Price DECIMAL(10,2) NULL CHECK (Price IS NULL OR Price >= 0),
    Status INT NOT NULL DEFAULT 1, -- Available/Held/Booked
    CONSTRAINT FK_Seats_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
    CONSTRAINT UQ_Seats_Event_SeatNumber UNIQUE (EventId, SeatNumber)
);
GO

CREATE TABLE ParkingSlots (
    ParkingSlotId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    SlotNumber NVARCHAR(20) NOT NULL,
    Zone NVARCHAR(50) NULL,
    Status INT NOT NULL DEFAULT 1, -- Available/Held/Reserved
    CONSTRAINT FK_ParkingSlots_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
    CONSTRAINT UQ_ParkingSlots_Event_Slot UNIQUE (EventId, SlotNumber)
);
GO

CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    BookingNumber NVARCHAR(30) NOT NULL UNIQUE,
    CustomerId INT NOT NULL,
    EventId INT NOT NULL,
    Status INT NOT NULL DEFAULT 1, -- Pending/Confirmed/Cancelled/Expired
    HoldExpiresAtUtc DATETIME2 NULL,
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    ConfirmedAtUtc DATETIME2 NULL,
    CancelledAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Bookings_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES Events(EventId)
);
GO

CREATE TABLE BookingSeats (
    BookingSeatId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    SeatId INT NOT NULL,
    CONSTRAINT FK_BookingSeats_Bookings FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE,
    CONSTRAINT FK_BookingSeats_Seats FOREIGN KEY (SeatId) REFERENCES Seats(SeatId),
    CONSTRAINT UQ_BookingSeats_Booking_Seat UNIQUE (BookingId, SeatId)
);
GO

CREATE TABLE ParkingReservations (
    ParkingReservationId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    ParkingSlotId INT NOT NULL,
    FeeAtReservation DECIMAL(10,2) NOT NULL CHECK (FeeAtReservation >= 0),
    IsActive BIT NOT NULL DEFAULT 1,
    ReservedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ReleasedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_ParkingReservations_Bookings FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE,
    CONSTRAINT FK_ParkingReservations_Slots FOREIGN KEY (ParkingSlotId) REFERENCES ParkingSlots(ParkingSlotId)
);
GO

CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL UNIQUE,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    Status INT NOT NULL DEFAULT 1,
    PaidAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE
);
GO

CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    Type INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Notifications_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Events_Venue_Date_Time ON Events(VenueId, EventDate, StartTime, EndTime);
CREATE INDEX IX_Bookings_Customer_Status ON Bookings(CustomerId, Status);
CREATE INDEX IX_Bookings_Event_Status ON Bookings(EventId, Status);
CREATE INDEX IX_Bookings_HoldExpiry ON Bookings(Status, HoldExpiresAtUtc);
CREATE INDEX IX_Notifications_Customer_Read ON Notifications(CustomerId, IsRead, CreatedAtUtc);
GO

/* ============================================================
   SAMPLE DATA — EXACTLY 150 ROWS
   Project-aligned demo/test data.
   Seeded Customer passwords: UNIQUE per customer (see login mapping below)
   Seeded Admin password:    Admin@123
   ============================================================ */

-- ============================================================
-- CUSTOMER LOGIN PASSWORDS (DEMO / TEST ONLY)
-- PasswordHash values below are ASP.NET Core Identity V3 hashes.
-- Use these REAL passwords when testing POST /api/auth/login:
-- 01. nimal.perera@example.com           Password: Nimal@101
-- 02. kavindi.silva@example.com          Password: Kavindi@102
-- 03. dinesh.fernando@example.com        Password: Dinesh@103
-- 04. tharushi.j@example.com             Password: Tharushi@104
-- 05. ashan.rodrigo@example.com          Password: Ashan@105
-- 06. dilani.kumari@example.com          Password: Dilani@106
-- 07. sahan.madushan@example.com         Password: Sahan@107
-- 08. piumi.hansika@example.com          Password: Piumi@108
-- 09. ravindu.s@example.com              Password: Ravindu@109
-- 10. ishara.lakmal@example.com          Password: Ishara@110
-- 11. shalini.peris@example.com          Password: Shalini@111
-- 12. kasun.bandara@example.com          Password: Kasun@112
-- 13. nadeesha.gamage@example.com        Password: Nadeesha@113
-- 14. chamod.w@example.com               Password: Chamod@114
-- 15. sachini.e@example.com              Password: Sachini@115
-- 16. malith.g@example.com               Password: Malith@116
-- 17. imesha.r@example.com               Password: Imesha@117
-- 18. dinuka.w@example.com               Password: Dinuka@118
-- 19. harini.a@example.com               Password: Harini@119
-- 20. vishwa.k@example.com               Password: Vishwa@120
-- ============================================================

-- 20 Customers
INSERT INTO Customers (Name, Email, Phone, PasswordHash, Status, EmailVerified, CreatedAtUtc)
VALUES
(N'Nimal Perera', N'nimal.perera@example.com', N'0771000001', N'AQAAAAIAAYagAAAAEOjZ+nii+3KwXvFctg7+q9D9E/zbCX3bZF0tC1t2B/wJFThlSOUXgqqtAAXwElUgMg==', 1, 1, DATEADD(DAY, -20, SYSUTCDATETIME())),
(N'Kavindi Silva', N'kavindi.silva@example.com', N'0771000002', N'AQAAAAIAAYagAAAAEPv6t0sSxbHmREUN3IFaW4UfggQJiEUySm91yG5s0AMYJs+YzqMMPm2dCFBlPWetAQ==', 1, 1, DATEADD(DAY, -19, SYSUTCDATETIME())),
(N'Dinesh Fernando', N'dinesh.fernando@example.com', N'0771000003', N'AQAAAAIAAYagAAAAEJicjZVqmiXWS7aKbLiQd8ejL8IOCvMiF1/WNbGOANGOyJF5AEs8yz1M2PmZ50QGKg==', 1, 1, DATEADD(DAY, -18, SYSUTCDATETIME())),
(N'Tharushi Jayasinghe', N'tharushi.j@example.com', N'0771000004', N'AQAAAAIAAYagAAAAEH+YWK5tMFkNL4uH1JXvF4taM/lnTSaa9IwnItV3OSUYwjpHQ5m11zvw0ay0Ovc/QA==', 1, 1, DATEADD(DAY, -17, SYSUTCDATETIME())),
(N'Ashan Rodrigo', N'ashan.rodrigo@example.com', N'0771000005', N'AQAAAAIAAYagAAAAEGJWBLMMJMI/kOlv/nWda7RFsij2Y5uZ+6vGof6dSz7EYEctxieudS67OAQT/ivb+A==', 1, 1, DATEADD(DAY, -16, SYSUTCDATETIME())),
(N'Dilani Kumari', N'dilani.kumari@example.com', N'0771000006', N'AQAAAAIAAYagAAAAEIEbpxv+xKfzBSbWfQP5LJGsF7IWSFizThlM4usflOSWiWqluCbsFAvagNxrgdxmsQ==', 1, 1, DATEADD(DAY, -15, SYSUTCDATETIME())),
(N'Sahan Madushan', N'sahan.madushan@example.com', N'0771000007', N'AQAAAAIAAYagAAAAEDdQSHsX4I/Qfa7TVrKzE5uIKtqkRdplLoYm8uynj8VU2td+ky/NDspGDB7lwyRJ4w==', 1, 1, DATEADD(DAY, -14, SYSUTCDATETIME())),
(N'Piumi Hansika', N'piumi.hansika@example.com', N'0771000008', N'AQAAAAIAAYagAAAAEDf2LKRM6XBw50uuDPoYgSs0i8XyUELBPpe1/tdinZcj0mutNG/pjTwpJMf7haL02Q==', 1, 1, DATEADD(DAY, -13, SYSUTCDATETIME())),
(N'Ravindu Senanayake', N'ravindu.s@example.com', N'0771000009', N'AQAAAAIAAYagAAAAEOEsFzP5za77XXn7ewEVaWvFOTB5S+ka1opgueGsO+FBAT5x4WqmjjwmrqdiLt2ZIw==', 1, 1, DATEADD(DAY, -12, SYSUTCDATETIME())),
(N'Ishara Lakmal', N'ishara.lakmal@example.com', N'0771000010', N'AQAAAAIAAYagAAAAEDZn5RwH2O04xUJAhPQXEdy1HT/FOGSh/hNx09ZJzfURx3T/8C6sLZgwacuegAdUIg==', 1, 1, DATEADD(DAY, -11, SYSUTCDATETIME())),
(N'Shalini Peris', N'shalini.peris@example.com', N'0712000011', N'AQAAAAIAAYagAAAAEOFITU7h2slz74i6cRIkwDrA18IkwsM5YTOUXUXTEi3FcdGZbrOR97JNeelKJOC50A==', 1, 1, DATEADD(DAY, -10, SYSUTCDATETIME())),
(N'Kasun Bandara', N'kasun.bandara@example.com', N'0712000012', N'AQAAAAIAAYagAAAAEGJJbP+oivsN3+v/bXQy8WbohV9ClABuhJ1TGamVNsuBVfibg/2Ce9NcLlwL4vx0nQ==', 1, 1, DATEADD(DAY, -9, SYSUTCDATETIME())),
(N'Nadeesha Gamage', N'nadeesha.gamage@example.com', N'0712000013', N'AQAAAAIAAYagAAAAEBz/HDWxSh4DFDcMqO5gBdoeyzwo8b5zEqrhxrL82zGnaqvC1asG4JvdvDNkli0kbA==', 1, 1, DATEADD(DAY, -8, SYSUTCDATETIME())),
(N'Chamod Wickramasinghe', N'chamod.w@example.com', N'0712000014', N'AQAAAAIAAYagAAAAEHtsYf4nTXOa6xlIG2xzbed8blVrsf92Ryh7BzHdZHqfDv3qJr9n6+N6W1Uo7xUK6Q==', 1, 1, DATEADD(DAY, -7, SYSUTCDATETIME())),
(N'Sachini Ekanayake', N'sachini.e@example.com', N'0712000015', N'AQAAAAIAAYagAAAAEBOGN4OK7XLcSarf9V0yG/cPsiEFhWQzHhpduKPsUXGS+64Q5knctFA9Lg5Rn6ACmA==', 1, 1, DATEADD(DAY, -6, SYSUTCDATETIME())),
(N'Malith Gunasekara', N'malith.g@example.com', N'0712000016', N'AQAAAAIAAYagAAAAELJWJUdt1+f81eS1UbABf35CV80sMVRvW0W80OP3xzTvBPYgV4G+YwuhisGwCD5OUg==', 1, 1, DATEADD(DAY, -5, SYSUTCDATETIME())),
(N'Imesha Rathnayake', N'imesha.r@example.com', N'0712000017', N'AQAAAAIAAYagAAAAECadclofbyxdWDFU4fVbV4xP7qh5yWnfcjolr6YGqPAfrKYlZdS0fOdWuMoyd+li2g==', 1, 1, DATEADD(DAY, -4, SYSUTCDATETIME())),
(N'Dinuka Weerasinghe', N'dinuka.w@example.com', N'0712000018', N'AQAAAAIAAYagAAAAEOyF+qVNUH4K657YOSOWkezByUOr7HZr04xioEcKzWRPUfqSqkXG1Set4+O4jm6MAA==', 1, 0, DATEADD(DAY, -3, SYSUTCDATETIME())),
(N'Harini Abeysekara', N'harini.a@example.com', N'0712000019', N'AQAAAAIAAYagAAAAEENdLw1iQpdkY99TlnYgXKn5zT628iSOKeZlGHYFOiNSZlsnvwyaKF1KolYb6D5/7w==', 2, 1, DATEADD(DAY, -2, SYSUTCDATETIME())),
(N'Vishwa Karunaratne', N'vishwa.k@example.com', N'0712000020', N'AQAAAAIAAYagAAAAENpXdMYhBdfMD2jtjB9D8l2bbQIFtioGzct8nHm/kzC8RgiE6PD4YIyi7fSzpfKMLQ==', 2, 1, DATEADD(DAY, -1, SYSUTCDATETIME()));
GO

-- 5 AdminUsers
INSERT INTO AdminUsers (Name, Email, PasswordHash, IsActive)
VALUES
(N'System Administrator', N'admin1@eventparking.local', N'AQAAAAIAAYagAAAAENn2YXqGtEwCdItBOrp8xT9GdyeyxsK97TklzWle7M+cjWK9K7FOTwfaLvSncwJpng==', 1),
(N'Event Administrator', N'admin2@eventparking.local', N'AQAAAAIAAYagAAAAENn2YXqGtEwCdItBOrp8xT9GdyeyxsK97TklzWle7M+cjWK9K7FOTwfaLvSncwJpng==', 1),
(N'Booking Administrator', N'admin3@eventparking.local', N'AQAAAAIAAYagAAAAENn2YXqGtEwCdItBOrp8xT9GdyeyxsK97TklzWle7M+cjWK9K7FOTwfaLvSncwJpng==', 1),
(N'Venue Administrator', N'admin4@eventparking.local', N'AQAAAAIAAYagAAAAENn2YXqGtEwCdItBOrp8xT9GdyeyxsK97TklzWle7M+cjWK9K7FOTwfaLvSncwJpng==', 1),
(N'Support Administrator', N'admin5@eventparking.local', N'AQAAAAIAAYagAAAAENn2YXqGtEwCdItBOrp8xT9GdyeyxsK97TklzWle7M+cjWK9K7FOTwfaLvSncwJpng==', 1);
GO

-- 5 Venues
INSERT INTO Venues (Name, Address, TotalCapacity)
VALUES
(N'Colombo Convention Centre', N'Colombo 01, Sri Lanka', 500),
(N'Nelum Hall', N'Colombo 07, Sri Lanka', 350),
(N'Kandy City Auditorium', N'Kandy, Sri Lanka', 250),
(N'Galle Event Arena', N'Galle, Sri Lanka', 300),
(N'Jaffna Cultural Hall', N'Jaffna, Sri Lanka', 220);
GO

-- 5 EventCategories
INSERT INTO EventCategories (Name)
VALUES
(N'Concert'),
(N'Sports'),
(N'Conference'),
(N'Workshop'),
(N'Exhibition');
GO

-- 10 Events
INSERT INTO Events (Name, VenueId, EventCategoryId, EventDate, StartTime, EndTime, TicketPrice, Capacity, ParkingFee)
VALUES
(N'Tech Future 2026', 1, 3, '2026-09-15', '09:00', '12:00', 2500.00, 4, 400.00),
(N'Colombo Music Night', 2, 1, '2026-09-18', '18:00', '22:00', 3500.00, 4, 500.00),
(N'Startup Workshop', 1, 4, '2026-09-20', '14:00', '17:00', 1800.00, 4, 300.00),
(N'City Sports Meetup', 3, 2, '2026-09-22', '10:00', '13:00', 1500.00, 4, 250.00),
(N'Education Expo', 4, 5, '2026-09-25', '09:00', '16:00', 1000.00, 4, 200.00),
(N'AI Conference Sri Lanka', 2, 3, '2026-10-02', '09:30', '15:30', 4000.00, 4, 500.00),
(N'Young Entrepreneurs Workshop', 5, 4, '2026-10-05', '13:00', '17:00', 2000.00, 4, 200.00),
(N'Acoustic Evening', 4, 1, '2026-10-08', '18:30', '21:30', 3000.00, 4, 350.00),
(N'Community Sports Day', 3, 2, '2026-10-12', '08:00', '12:00', 1200.00, 4, 250.00),
(N'Innovation Exhibition', 5, 5, '2026-10-15', '09:00', '17:00', 900.00, 4, 200.00);
GO

-- 40 Seats: 4 per event, matching each Event.Capacity = 4
INSERT INTO Seats (EventId, SeatNumber, SeatType, Price, Status)
VALUES
(1, N'A1', N'Standard', NULL, 3),
(1, N'A2', N'Standard', NULL, 1),
(1, N'B1', N'Standard', NULL, 1),
(1, N'B2', N'Standard', NULL, 1),
(2, N'A1', N'Standard', NULL, 2),
(2, N'A2', N'Standard', NULL, 1),
(2, N'B1', N'Standard', NULL, 1),
(2, N'B2', N'Standard', NULL, 1),
(3, N'A1', N'Standard', NULL, 1),
(3, N'A2', N'Standard', NULL, 1),
(3, N'B1', N'Standard', NULL, 1),
(3, N'B2', N'Standard', NULL, 1),
(4, N'A1', N'Standard', NULL, 1),
(4, N'A2', N'Standard', NULL, 1),
(4, N'B1', N'Standard', NULL, 1),
(4, N'B2', N'Standard', NULL, 1),
(5, N'A1', N'Standard', NULL, 3),
(5, N'A2', N'Standard', NULL, 1),
(5, N'B1', N'Standard', NULL, 1),
(5, N'B2', N'Standard', NULL, 1),
(6, N'A1', N'Standard', NULL, 3),
(6, N'A2', N'Standard', NULL, 1),
(6, N'B1', N'Standard', NULL, 1),
(6, N'B2', N'Standard', NULL, 1),
(7, N'A1', N'Standard', NULL, 2),
(7, N'A2', N'Standard', NULL, 1),
(7, N'B1', N'Standard', NULL, 1),
(7, N'B2', N'Standard', NULL, 1),
(8, N'A1', N'Standard', NULL, 1),
(8, N'A2', N'Standard', NULL, 1),
(8, N'B1', N'Standard', NULL, 1),
(8, N'B2', N'Standard', NULL, 1),
(9, N'A1', N'Standard', NULL, 1),
(9, N'A2', N'Standard', NULL, 1),
(9, N'B1', N'Standard', NULL, 1),
(9, N'B2', N'Standard', NULL, 1),
(10, N'A1', N'Standard', NULL, 3),
(10, N'A2', N'Standard', NULL, 1),
(10, N'B1', N'Standard', NULL, 1),
(10, N'B2', N'Standard', NULL, 1);
GO

-- 20 ParkingSlots: 2 per event
INSERT INTO ParkingSlots (EventId, SlotNumber, Zone, Status)
VALUES
(1, N'P1', N'A', 3),
(1, N'P2', N'B', 1),
(2, N'P1', N'A', 2),
(2, N'P2', N'B', 1),
(3, N'P1', N'A', 1),
(3, N'P2', N'B', 1),
(4, N'P1', N'A', 1),
(4, N'P2', N'B', 1),
(5, N'P1', N'A', 3),
(5, N'P2', N'B', 1),
(6, N'P1', N'A', 1),
(6, N'P2', N'B', 1),
(7, N'P1', N'A', 2),
(7, N'P2', N'B', 1),
(8, N'P1', N'A', 1),
(8, N'P2', N'B', 1),
(9, N'P1', N'A', 1),
(9, N'P2', N'B', 1),
(10, N'P1', N'A', 1),
(10, N'P2', N'B', 1);
GO

-- 10 Bookings
INSERT INTO Bookings (BookingNumber, CustomerId, EventId, Status, HoldExpiresAtUtc, TotalAmount, CreatedAtUtc, UpdatedAtUtc, ConfirmedAtUtc, CancelledAtUtc)
VALUES
(N'BKG-2026-000001', 1, 1, 2, NULL, 2900.00, '2026-09-01T08:00:00', '2026-09-01T08:05:00', '2026-09-01T08:05:00', NULL),
(N'BKG-2026-000002', 2, 2, 1, '2026-09-01T21:00:00', 4000.00, '2026-09-01T20:45:00', NULL, NULL, NULL),
(N'BKG-2026-000003', 3, 3, 3, NULL, 2100.00, '2026-08-28T10:00:00', '2026-08-28T10:30:00', '2026-08-28T10:05:00', '2026-08-29T09:00:00'),
(N'BKG-2026-000004', 4, 4, 4, NULL, 1500.00, '2026-08-27T09:00:00', '2026-08-27T09:20:00', NULL, NULL),
(N'BKG-2026-000005', 5, 5, 2, NULL, 1200.00, '2026-09-01T09:00:00', '2026-09-01T09:10:00', '2026-09-01T09:10:00', NULL),
(N'BKG-2026-000006', 6, 6, 2, NULL, 4000.00, '2026-09-01T10:00:00', '2026-09-01T10:08:00', '2026-09-01T10:08:00', NULL),
(N'BKG-2026-000007', 7, 7, 1, '2026-09-01T21:10:00', 2200.00, '2026-09-01T20:55:00', NULL, NULL, NULL),
(N'BKG-2026-000008', 8, 8, 3, NULL, 3000.00, '2026-08-30T15:00:00', '2026-08-30T15:15:00', NULL, '2026-08-30T16:00:00'),
(N'BKG-2026-000009', 9, 9, 4, NULL, 1200.00, '2026-08-29T12:00:00', '2026-08-29T12:20:00', NULL, NULL),
(N'BKG-2026-000010', 10, 10, 2, NULL, 900.00, '2026-09-01T11:00:00', '2026-09-01T11:05:00', '2026-09-01T11:05:00', NULL);
GO

-- 10 BookingSeats
INSERT INTO BookingSeats (BookingId, SeatId)
VALUES
(1, 1),
(2, 5),
(3, 9),
(4, 13),
(5, 17),
(6, 21),
(7, 25),
(8, 29),
(9, 33),
(10, 37);
GO

-- 5 ParkingReservations
INSERT INTO ParkingReservations (BookingId, ParkingSlotId, FeeAtReservation, IsActive, ReservedAtUtc, ReleasedAtUtc)
VALUES
(1, 1, 400.00, 1, '2026-09-01T08:00:00', NULL),
(2, 3, 500.00, 1, '2026-09-01T20:45:00', NULL),
(3, 5, 300.00, 0, '2026-08-28T10:00:00', '2026-08-29T09:00:00'),
(5, 9, 200.00, 1, '2026-09-01T09:00:00', NULL),
(7, 13, 200.00, 1, '2026-09-01T20:55:00', NULL);
GO

-- 5 Payments
INSERT INTO Payments (BookingId, Amount, Status, PaidAtUtc)
VALUES
(1, 2900.00, 1, '2026-09-01T08:05:00'),
(3, 2100.00, 1, '2026-08-28T10:05:00'),
(5, 1200.00, 1, '2026-09-01T09:10:00'),
(6, 4000.00, 1, '2026-09-01T10:08:00'),
(10, 900.00, 1, '2026-09-01T11:05:00');
GO

-- 15 Notifications
INSERT INTO Notifications (CustomerId, Type, Message, IsRead, CreatedAtUtc)
VALUES
(1, 4, N'Payment completed for booking BKG-2026-000001.', 0, '2026-09-01T08:05:00'),
(1, 1, N'Booking BKG-2026-000001 is confirmed.', 1, '2026-09-01T08:05:05'),
(2, 5, N'Your booking BKG-2026-000002 is awaiting payment.', 0, '2026-09-01T20:46:00'),
(3, 2, N'Booking BKG-2026-000003 was cancelled.', 1, '2026-08-29T09:00:00'),
(4, 3, N'Booking BKG-2026-000004 expired.', 0, '2026-08-27T09:20:00'),
(5, 1, N'Booking BKG-2026-000005 is confirmed.', 0, '2026-09-01T09:10:00'),
(6, 4, N'Payment completed for booking BKG-2026-000006.', 1, '2026-09-01T10:08:00'),
(6, 1, N'Booking BKG-2026-000006 is confirmed.', 0, '2026-09-01T10:08:05'),
(7, 5, N'Complete payment before your booking hold expires.', 0, '2026-09-01T20:56:00'),
(8, 2, N'Booking BKG-2026-000008 was cancelled.', 1, '2026-08-30T16:00:00'),
(9, 3, N'Booking BKG-2026-000009 expired.', 0, '2026-08-29T12:20:00'),
(10, 1, N'Booking BKG-2026-000010 is confirmed.', 0, '2026-09-01T11:05:00'),
(11, 6, N'Event information has been updated.', 0, '2026-09-01T12:00:00'),
(12, 5, N'New events are available for booking.', 0, '2026-09-01T12:10:00'),
(13, 5, N'Remember to verify event details before booking.', 1, '2026-09-01T12:20:00');
GO

-- Row-count summary
-- Customers: 20
-- AdminUsers: 5
-- Venues: 5
-- EventCategories: 5
-- Events: 10
-- Seats: 40
-- ParkingSlots: 20
-- Bookings: 10
-- BookingSeats: 10
-- ParkingReservations: 5
-- Payments: 5
-- Notifications: 15
-- TOTAL: 150

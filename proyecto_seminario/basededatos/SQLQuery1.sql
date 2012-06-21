create database BibliotecaColectiva
go
use BibliotecaColectiva


create table Usuario (
	IdUsuario uniqueidentifier primary key,
	ApellidoPat varchar (15) default '',
	ApellidoMat varchar (15) not null default '',
	Nombre varchar (15) default '',
	Email varchar (50) not null,
	Contrasena varchar (max) not null,
	NickName varchar (15),
	Avatar varchar(30),
	Ubicacion varchar (30),
	Intereses varchar (50),
	Karma float
)


go


create table Categoria(
	IdCategoria int identity (1,1) primary key,
	Nombre varchar (30) not null
)

go


create table Publicacion(
	IdPublicacion int identity(1,1) primary key,
	Titulo varchar (30) not null,
	Descripcion varchar (50) not null,
	Id_Cat int foreign key references Categoria(IdCategoria)
)
go

create table Articulo(
	IdPublicacion int foreign key references Publicacion(IdPublicacion),
	Tema varchar(50),
	MeGusta int
)
go

Create table Curso(
	IdPublicacion int  foreign key references Publicacion(IdPublicacion),
	Tema varchar(60),
	MeGusta int
)
go

Create table Tutorial(
	IdPublicacion int  foreign key references Publicacion(IdPublicacion),
	Tema varchar(150),
	MeGusta int
)
go

Create table Libro(
	IdPublicacion int foreign key references Publicacion(IdPublicacion),
	Tema varchar(50),
	Indice varchar(50),
	Portada varchar(50),
	Autor varchar (30),
	AnoPublicacion date,
	MeGusta int
)
go

create table Comentario(
	Id_Com int identity (1,1) primary key,
	Id_Us uniqueidentifier foreign key references Usuario(IdUsuario),
	Id_Pub int foreign key references Publicacion(IdPublicacion),
	Texto varchar(50),
	Fecha date
)
go

create table Karma(
	Id_Us uniqueidentifier foreign key references Usuario(IdUsuario),
	IdPub int foreign key references Publicacion(IdPublicacion )
)
go



select * from Usuario
select * from aspnet_Membership
select * from Publicacion
select * from Libro
select * from Categoria

create view Info_Libro as
	select p.IdPublicacion,p.Titulo,p.Descripcion,l.Portada,l.MeGusta,cat.Nombre
	from Publicacion p, Categoria  cat,Libro l
	where l.IdPublicacion=p.IdPublicacion and p.Id_Cat=cat.IdCategoria
	
go
insert into Categoria values('Ciencia y tecnologia')
insert into Categoria values('Deportes')

 insert into Publicacion values('Guia de musculacion','Una guia completa sobre rutinas de musculacion',2)
 insert into Publicacion values('MVC 3.0','Manual basico de MVC 3.0 en ingles',1)
 
 
 insert into Libro values(3,'Aqui va la ruta','otra ruta','contenidos/IMG0001.jpeg','Ni idea','1989-01-01',0)
 insert into Libro values(4,'Aqui va la ruta','otra ruta','contenidos/IMG0002.jpeg','Ni idea','1989-01-01',0)
 
 
 
 
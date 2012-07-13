create database Biblioteca
go
use Biblioteca

create table diccionario(
	id int identity primary key,
	palabra varchar(128)
)
go
create table Perfil_Usuario(
	Id int identity(1,1) primary key,
	Nickname varchar(30),
	Nombre varchar(30),
	Apellidos varchar(40),
	Intereses varchar(120),
	Avatar varchar(100) default 'Avatar/default.jpg',
	Ubicacion varchar(50),
	
	Id_User uniqueidentifier foreign key references aspnet_Users(UserId),
	Karma float default 0,
	estado int
)
go



create trigger Insercion_perfil
on aspnet_Users
after insert 
as
	declare @id uniqueidentifier
	set @id=(select UserId from inserted)
	insert into Perfil_Usuario values(null,null,null,null,default,null,@id,0,1)
go


create trigger Borrado
on aspnet_Users
instead of delete
as
	declare @id uniqueidentifier
	set @id=(select UserId from deleted)
	delete from Perfil_Usuario where Id_User=@id
	delete from aspnet_Users where UserId=@id
go

create table Categoria(
	Id_cat int identity(1,1) primary key,
	Nombre varchar(80)
)
go
create table Rel_Categoria_Contenido(
	IdContenido int foreign key references Contenido(IdContenido),
	Id_cat int foreign key references Categoria(Id_cat),
	primary key(IdContenido,Id_cat)
)
go




create table Contenido(
	IdContenido int identity(1,1) primary key,
	Id_User uniqueidentifier foreign key references aspnet_Users(UserId),
	Titulo varchar (30) ,
	Descripcion varchar (200) ,
	Tipo varchar(15),
	check (Tipo in('Articulo','Curso','Tutorial','Libro')),
	Fecha_publicacion date default getdate(),
	estado int,
	observaciones varchar(128)
)
go

create trigger act_contenido
on contenido
after update
as
	declare @tipo varchar(15)
	declare @estado int
	declare @id uniqueidentifier
	set @estado=(select estado from inserted)
	set @tipo=(select Tipo from inserted)
	set @id=(select Id_user from inserted)
	if @estado=1
	begin
		if(@tipo='Libro')
			set @estado=100
		if(@tipo='Articulo')
			set @estado=10
		if(@tipo='Tutorial')
			set @estado=50
		if(@tipo='Curso')
			set @estado=200
		update perfil_usuario set karma=karma+@estado where Id_User=@id
	end	
go

create table Publicacion(
	IdPublicacion int foreign key references Contenido(IdContenido),
	Tema text	
	primary key(IdPublicacion)
)
go
select * from contenido


Create table Libro(
	IdPublicacion int foreign key references Contenido(IdContenido),
	Portada varchar(50),
	Tema varchar(50),
	Autor varchar (30),
	AnoPublicacion date,
	primary key(IdPublicacion)
)
go

--insert into Contenido values('54E3B906-48F7-4B57-AEAC-B33FF7308A18','????','????',1,'Libro')
insert into Libro values (13,'','','/contenidos/IMG00002.jpg','desc','1989-01-01')
select * from contenido

select * from Libro
select * from Gusta
select * from Categoria
select * from perfil_usuario
select * from Contenido inner join Perfil_Usuario on Contenido.Id_User=Perfil_Usuario.Id_User

create table Comentario(
	Id_Com int identity (1,1) primary key,
	Id_Us uniqueidentifier foreign key references aspnet_Users(Userid),
	Id_Cont int foreign key references Contenido(IdContenido),
	Texto text,
	Fecha date
)
go

create table Gusta(
Id_Us uniqueidentifier foreign key references aspnet_Users(UserId),
	IdPub int foreign key references contenido(Idcontenido )
	primary key(Id_Us,IdPub)
)
go
create view perfil as
	select u.UserName, mem.UserId,p.Nickname,p.Apellidos,p.Nombre,p.Intereses,p.Avatar,p.Ubicacion,convert(int,(p.Karma)) as karma,mem.Email
	from aspnet_Membership mem,aspnet_Users u,Perfil_Usuario p
	where mem.UserId=u.UserId and p.Id_User=u.UserId
	
go
create view VistaGeneralLibro as
	select C.IdContenido,C.Titulo,C.Descripcion,l.Portada,pu.Avatar,pu.Nickname,pu.Karma,C.estado
	from Contenido C,Libro l, Perfil_Usuario pu
	where C.IdContenido=l.IdPublicacion and C.Id_User=pu.Id_User 
	
create view VistaGeneralPublicacion as
	select C.IdContenido,C.Titulo,C.Descripcion,pu.Avatar,pu.Nickname,pu.Karma,C.Tipo,C.estado
	from Contenido C,Publicacion P, Perfil_Usuario pu
	where C.IdContenido=P.IdPublicacion and C.Id_User=pu.Id_User 
select * from VistaGeneralPublicacion

create view Cant_Gusta as
	select count(Id_Us)as 'cantidad',IdPub from Gusta	
	group by IdPub
go

create trigger mas_comentario on Comentario
after insert
as
	declare @id uniqueidentifier
	set @id=(select Id_Us from inserted)
	update perfil_usuario set karma=karma+0.5 where Id_User=@id
go	

create trigger menos_comentario on Comentario
after delete
as
	update perfil_usuario set karma=karma-0.5 where Id_User in (select Id_Us from deleted)
go	

create trigger aum_karma on Gusta
after insert as
	declare @id int
	set @id=(select IdPub from inserted)
	declare @us uniqueidentifier
	set @us=(select Id_User from contenido where IdContenido=@id)
	update perfil_usuario set karma=karma+1 where id_user=@us
go
create trigger dism_karma on Gusta
after delete as
	declare @id int
	set @id=(select IdPub from deleted)
	declare @us uniqueidentifier
	set @us=(select Id_User from contenido where IdContenido=@id)
	update perfil_usuario set karma=karma-1 where id_user=@us
go	


create view ListaCatego as
	select r.IdContenido,c.Nombre
	from Rel_Categoria_Contenido r,Categoria c
	where r.Id_cat=c.Id_cat	
go

create view vista_comentario
as
	select c.Id_Com,c.Texto,c.Id_Us,pu.Avatar,pu.Karma
	from Comentario c,Perfil_Usuario pu
	where c.Id_Us = pu.Id_User
go


use biblioteca

select * from Gusta
select * from Perfil_Usuario
select * from aspnet_Membership
select * from aspnet_Users
select * from aspnet_UsersInRoles
select * from aspnet_Roles
select * from VistaGeneralLibro
select * from contenido
select * from publicacion
select * from libro


select * from Perfil_Usuario
select * from comentario
select * from Categoria
select * from Rel_Categoria_Contenido
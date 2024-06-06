-- Crear el usuario USER_VENTAS con la contraseña 1234
CREATE USER USER_VENTAS IDENTIFIED BY 1234;

-- Permitir la ejecución de scripts administrativos
ALTER SESSION SET "_ORACLE_SCRIPT"=true;

-- Otorgar privilegios necesarios al usuario USER_VENTAS
GRANT CREATE SESSION TO USER_VENTAS;
GRANT CREATE TABLE TO USER_VENTAS;
GRANT CREATE VIEW TO USER_VENTAS;
GRANT CREATE PROCEDURE TO USER_VENTAS;
GRANT CREATE TRIGGER TO USER_VENTAS;
GRANT CREATE SEQUENCE TO USER_VENTAS;
GRANT CREATE JOB TO USER_VENTAS;

-- Configurar la cuota de espacio en el tablespace USERS para USER_VENTAS
ALTER USER USER_VENTAS QUOTA UNLIMITED ON USERS;


--Creacion de tabla PRODUCTOS

CREATE TABLE PRODUCTOS (
    ID_PRODUCTO NUMBER GENERATED ALWAYS AS IDENTITY,
    NOMBRE VARCHAR2(100) NOT NULL,
    DESCRIPCION VARCHAR2(200),
    PRECIO NUMBER(10,2) NOT NULL,
    STOCK NUMBER NOT NULL,
    FECHA_CREACION DATE DEFAULT SYSDATE,
    FECHA_MODIFICACION DATE,
    CONSTRAINT PK_PRODUCTOS PRIMARY KEY (ID_PRODUCTO),
    CONSTRAINT CK_PRODUCTOS_PRECIO CHECK (PRECIO >= 0),
    CONSTRAINT CK_PRODUCTOS_STOCK CHECK (STOCK >= 0)
);

--Creacion de tabla CLIENTES

CREATE TABLE CLIENTES (
    ID_CLIENTE NUMBER GENERATED ALWAYS AS IDENTITY,
    NOMBRE VARCHAR2(100) NOT NULL,
    APELLIDO VARCHAR2(100) NOT NULL,
    EMAIL VARCHAR2(100) UNIQUE,
    TELEFONO VARCHAR2(20),
    DIRECCION VARCHAR2(200),
    FECHA_CREACION DATE DEFAULT SYSDATE,
    FECHA_MODIFICACION DATE,
    CONSTRAINT PK_CLIENTES PRIMARY KEY (ID_CLIENTE)
);

--Creacion de tabla VENTAS

CREATE TABLE VENTAS (
    ID_VENTA NUMBER GENERATED ALWAYS AS IDENTITY,
    ID_CLIENTE NUMBER NOT NULL,
    FECHA_VENTA DATE DEFAULT SYSDATE,
    TOTAL NUMBER(10,2) NOT NULL,
    ESTADO VARCHAR2(20) DEFAULT 'PENDIENTE',
    FECHA_CREACION DATE DEFAULT SYSDATE,
    FECHA_MODIFICACION DATE,
    CONSTRAINT PK_VENTAS PRIMARY KEY (ID_VENTA),
    CONSTRAINT FK_VENTAS_CLIENTES FOREIGN KEY (ID_CLIENTE) REFERENCES CLIENTES(ID_CLIENTE),
    CONSTRAINT CK_VENTAS_TOTAL CHECK (TOTAL >= 0),
    CONSTRAINT CK_VENTAS_ESTADO CHECK (ESTADO IN ('PENDIENTE', 'COMPLETADA', 'CANCELADA'))
);

--Creacion de tabla DETALLE_VENTAS

CREATE TABLE DETALLE_VENTAS (
    ID_DETALLE NUMBER GENERATED ALWAYS AS IDENTITY,
    ID_VENTA NUMBER NOT NULL,
    ID_PRODUCTO NUMBER NOT NULL,
    CANTIDAD NUMBER NOT NULL,
    PRECIO_UNITARIO NUMBER(10,2) NOT NULL,
    SUBTOTAL NUMBER(10,2) GENERATED ALWAYS AS (CANTIDAD * PRECIO_UNITARIO),
    CONSTRAINT PK_DETALLE_VENTAS PRIMARY KEY (ID_DETALLE),
    CONSTRAINT FK_DETALLE_VENTAS_VENTAS FOREIGN KEY (ID_VENTA) REFERENCES VENTAS(ID_VENTA),
    CONSTRAINT FK_DETALLE_VENTAS_PRODUCTOS FOREIGN KEY (ID_PRODUCTO) REFERENCES PRODUCTOS(ID_PRODUCTO),
    CONSTRAINT CK_DETALLE_VENTAS_CANTIDAD CHECK (CANTIDAD > 0),
    CONSTRAINT CK_DETALLE_VENTAS_PRECIO CHECK (PRECIO_UNITARIO >= 0)
);

-- Procedimientos Almacenados

--Procedimiento Almacenado para el ABM de Productos


CREATE OR REPLACE  PROCEDURE SP_ABM_PRODUCTOS (
    P_ACCION IN VARCHAR2,
    P_ID_PRODUCTO IN PRODUCTOS.ID_PRODUCTO%TYPE,
    P_NOMBRE IN PRODUCTOS.NOMBRE%TYPE,
    P_DESCRIPCION IN PRODUCTOS.DESCRIPCION%TYPE,
    P_PRECIO IN PRODUCTOS.PRECIO%TYPE,
    P_STOCK IN PRODUCTOS.STOCK%TYPE
)
AS
BEGIN

    -- Se valida que el nombre del producto sea requerido, pero solo para altas y modificaciones.
    IF P_ACCION <> 'B' AND (P_NOMBRE IS NULL OR P_NOMBRE = '') THEN
        RAISE_APPLICATION_ERROR(-20001, 'El nombre del producto es requerido.');
    END IF;

    -- Validamos que el precio del producto sea mayor o igual a cero.

    IF P_PRECIO < 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'El precio del producto debe ser mayor o igual a cero.');
    END IF;

    -- Tambien se valida que el stock del producto sea mayor o igual a cero.

    IF P_STOCK < 0 THEN
        RAISE_APPLICATION_ERROR(-20003, 'El stock del producto debe ser mayor o igual a cero.');
    END IF;

    -- Ahora dependiendo de la accion que se pase, vamos a realizar la operacion correspondiente.
    IF P_ACCION = 'A' THEN
        -- Si la accion es 'A', dar de alta un nuevo producto.
        -- Insertamos los datos del producto en la tabla.

        INSERT INTO PRODUCTOS (NOMBRE, DESCRIPCION, PRECIO, STOCK)
        VALUES (P_NOMBRE, P_DESCRIPCION, P_PRECIO, P_STOCK);
    ELSIF P_ACCION = 'B' THEN
        -- Si la accion es 'B', dar de baja al producto existente.
        -- Se borra el producto de la tabla usando el ID que se paso.
        DELETE FROM PRODUCTOS WHERE ID_PRODUCTO = P_ID_PRODUCTO;
    ELSIF P_ACCION = 'M' THEN
        -- Si la accion es 'M', modificar un producto existente.
        -- Actualizamos los datos del producto en la tabla usando el ID que se paso.
        -- Tambien se actualiza la fecha de modificacion con la fecha actual.
        UPDATE PRODUCTOS
        SET NOMBRE = P_NOMBRE,
            DESCRIPCION = P_DESCRIPCION,
            PRECIO = P_PRECIO,
            STOCK = P_STOCK,
            FECHA_MODIFICACION = SYSDATE
        WHERE ID_PRODUCTO = P_ID_PRODUCTO;
    ELSE
        -- Si la accion no es 'A', 'B' o 'M', mostrar error.

        RAISE_APPLICATION_ERROR(-20004, 'Accion no valida. Debe ser A (Alta), B (Baja) o M (Modificacion).');
    END IF;

    -- Si todo sale bien, hacemos un COMMIT para confirmar los cambios en la base de datos.
    COMMIT;
EXCEPTION
    --  Hacemos un ROLLBACK para deshacer los cambios si algo salio mal
    -- Despues relanzamos la excepcion para indicar que algo fallo.
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;




--Procedimiento Almacenado para Listar Productos

CREATE OR REPLACE  PROCEDURE SP_LISTAR_PRODUCTOS (
    P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR
    SELECT ID_PRODUCTO, NOMBRE, DESCRIPCION, PRECIO, STOCK
    FROM PRODUCTOS;
END;


--Procedimiento Almacenado para Listar Clientes

CREATE OR REPLACE NONEDITIONABLE PROCEDURE SP_LISTAR_CLIENTES (
    P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR
    SELECT ID_CLIENTE, NOMBRE, APELLIDO, EMAIL, TELEFONO, DIRECCION
    FROM CLIENTES;
END;

--Procedimiento Almacenado para ABM de Cliente


CREATE OR REPLACE  PROCEDURE SP_ABM_CLIENTES (
    P_ACCION IN VARCHAR2,
    P_ID_CLIENTE IN CLIENTES.ID_CLIENTE%TYPE,
    P_NOMBRE IN CLIENTES.NOMBRE%TYPE,
    P_APELLIDO IN CLIENTES.APELLIDO%TYPE,
    P_EMAIL IN CLIENTES.EMAIL%TYPE,
    P_TELEFONO IN CLIENTES.TELEFONO%TYPE,
    P_DIRECCION IN CLIENTES.DIRECCION%TYPE
)
AS
BEGIN
    -- Validar que el nombre y el apellido del cliente sean requeridos en altas y modificaciones.
    IF P_ACCION <> 'B' AND (P_NOMBRE IS NULL OR P_NOMBRE = '' OR P_APELLIDO IS NULL OR P_APELLIDO = '') THEN
        RAISE_APPLICATION_ERROR(-20001, 'El nombre y el apellido del cliente son requeridos.');
    END IF;

    -- Dependiendo de la accion se va realizar la operacion correspondiente.
    IF P_ACCION = 'A' THEN
        -- Si accion es 'A', dar de alta un nuevo cliente.
        -- Insertamos los datos del cliente en la tabla.
        INSERT INTO CLIENTES (NOMBRE, APELLIDO, EMAIL, TELEFONO, DIRECCION, FECHA_CREACION)
        VALUES (P_NOMBRE, P_APELLIDO, P_EMAIL, P_TELEFONO, P_DIRECCION, SYSDATE);
    ELSIF P_ACCION = 'B' THEN
        -- Si la accion es 'B', dar de baja al cliente existente.
        DELETE FROM CLIENTES WHERE ID_CLIENTE = P_ID_CLIENTE;
    ELSIF P_ACCION = 'M' THEN
        -- Si la accion es 'M' modificar el cliente existente
        -- Se actualiza los datos del cliente en la tabla.
        -- Tambien se actualiza la fecha de modificación con la fecha actual.
        UPDATE CLIENTES
        SET
            NOMBRE = P_NOMBRE,
            APELLIDO = P_APELLIDO,
            EMAIL = P_EMAIL,
            TELEFONO = P_TELEFONO,
            DIRECCION = P_DIRECCION,
            FECHA_MODIFICACION = SYSDATE
        WHERE ID_CLIENTE = P_ID_CLIENTE;
    ELSE
        -- Si la accion no es 'A', 'B' o 'M', mostrar error.
        RAISE_APPLICATION_ERROR(-20002, 'Accion no valida. Debe ser A (Alta), B (Baja) o M (Modificacion).');
    END IF;

    -- Si todo esta bien hacemos un COMMIT para confirmar los cambios en la base de datos.
    COMMIT;
EXCEPTION
    -- Si salio mal algo hacemos un ROLLBACK para deshacer los cambios.
    -- Despues relanzamos la excepcion para indicar que algo fallo.
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;




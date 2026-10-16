CREATE DATABASE  IF NOT EXISTS `farmacia` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `farmacia`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: farmacia
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `citas`
--

DROP TABLE IF EXISTS `citas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `citas` (
  `id_Cita` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Estatus` int unsigned NOT NULL,
  `id_Paciente` int unsigned NOT NULL,
  `id_Medico` int unsigned NOT NULL,
  `hora_Cita` time NOT NULL,
  `fecha_Cita` date NOT NULL,
  PRIMARY KEY (`id_Cita`),
  KEY `fk_citas_estatus` (`id_Estatus`),
  KEY `fk_citas_paciente` (`id_Paciente`),
  KEY `fk_citas_medico` (`id_Medico`),
  CONSTRAINT `fk_citas_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`),
  CONSTRAINT `fk_citas_medico` FOREIGN KEY (`id_Medico`) REFERENCES `medicos` (`id_Medico`),
  CONSTRAINT `fk_citas_paciente` FOREIGN KEY (`id_Paciente`) REFERENCES `pacientes` (`id_Paciente`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `citas`
--

LOCK TABLES `citas` WRITE;
/*!40000 ALTER TABLE `citas` DISABLE KEYS */;
/*!40000 ALTER TABLE `citas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compras`
--

DROP TABLE IF EXISTS `compras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compras` (
  `id_Compra` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Estatus` int unsigned NOT NULL,
  `id_Proveedor` int unsigned NOT NULL,
  `fecha_Compra` date NOT NULL,
  `hora_Compra` time DEFAULT NULL,
  `total_Compra` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id_Compra`),
  KEY `fk_compras_estatus` (`id_Estatus`),
  KEY `fk_compras_proveedor` (`id_Proveedor`),
  CONSTRAINT `fk_compras_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`),
  CONSTRAINT `fk_compras_proveedor` FOREIGN KEY (`id_Proveedor`) REFERENCES `proveedores` (`id_Proveedor`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compras`
--

LOCK TABLES `compras` WRITE;
/*!40000 ALTER TABLE `compras` DISABLE KEYS */;
/*!40000 ALTER TABLE `compras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `consultas`
--

DROP TABLE IF EXISTS `consultas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `consultas` (
  `id_Consulta` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Estatus` int unsigned NOT NULL,
  `id_Cita` int unsigned NOT NULL,
  `sintomas_Consulta` text NOT NULL,
  `diagnostico_Consulta` text NOT NULL,
  `observaciones_Consulta` text,
  `peso` double NOT NULL,
  `altura` double NOT NULL,
  PRIMARY KEY (`id_Consulta`),
  KEY `fk_consultas_estatus` (`id_Estatus`),
  KEY `fk_consultas_cita` (`id_Cita`),
  CONSTRAINT `fk_consultas_cita` FOREIGN KEY (`id_Cita`) REFERENCES `citas` (`id_Cita`),
  CONSTRAINT `fk_consultas_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `consultas`
--

LOCK TABLES `consultas` WRITE;
/*!40000 ALTER TABLE `consultas` DISABLE KEYS */;
/*!40000 ALTER TABLE `consultas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle_compra`
--

DROP TABLE IF EXISTS `detalle_compra`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle_compra` (
  `id_Detalle_Compra` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Compra` int unsigned NOT NULL,
  `id_Medicamento` int unsigned NOT NULL,
  `cantidad` int unsigned NOT NULL,
  `precio_unitario` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id_Detalle_Compra`,`id_Compra`,`id_Medicamento`),
  KEY `fk_dc_compra` (`id_Compra`),
  KEY `fk_dc_medicamento` (`id_Medicamento`),
  CONSTRAINT `fk_dc_compra` FOREIGN KEY (`id_Compra`) REFERENCES `compras` (`id_Compra`),
  CONSTRAINT `fk_dc_medicamento` FOREIGN KEY (`id_Medicamento`) REFERENCES `medicamentos` (`id_Medicamento`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle_compra`
--

LOCK TABLES `detalle_compra` WRITE;
/*!40000 ALTER TABLE `detalle_compra` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalle_compra` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle_receta`
--

DROP TABLE IF EXISTS `detalle_receta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle_receta` (
  `id_detalle_receta` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Medicamento` int unsigned NOT NULL,
  `id_Consulta` int unsigned NOT NULL,
  `duracion` varchar(20) NOT NULL,
  `frecuencia` varchar(20) NOT NULL,
  `dosis` varchar(20) NOT NULL,
  PRIMARY KEY (`id_detalle_receta`,`id_Medicamento`,`id_Consulta`),
  KEY `fk_dr_medicamento` (`id_Medicamento`),
  KEY `fk_dr_consulta` (`id_Consulta`),
  CONSTRAINT `fk_dr_consulta` FOREIGN KEY (`id_Consulta`) REFERENCES `consultas` (`id_Consulta`),
  CONSTRAINT `fk_dr_medicamento` FOREIGN KEY (`id_Medicamento`) REFERENCES `medicamentos` (`id_Medicamento`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle_receta`
--

LOCK TABLES `detalle_receta` WRITE;
/*!40000 ALTER TABLE `detalle_receta` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalle_receta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle_venta`
--

DROP TABLE IF EXISTS `detalle_venta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle_venta` (
  `id_Detalle_Venta` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Venta` int unsigned NOT NULL,
  `id_Medicamento` int unsigned NOT NULL,
  `cantidad` int unsigned NOT NULL,
  PRIMARY KEY (`id_Detalle_Venta`,`id_Venta`,`id_Medicamento`),
  KEY `fk_dv_venta` (`id_Venta`),
  KEY `fk_dv_medicamento` (`id_Medicamento`),
  CONSTRAINT `fk_dv_medicamento` FOREIGN KEY (`id_Medicamento`) REFERENCES `medicamentos` (`id_Medicamento`),
  CONSTRAINT `fk_dv_venta` FOREIGN KEY (`id_Venta`) REFERENCES `ventas` (`id_Venta`)
) ENGINE=InnoDB AUTO_INCREMENT=117 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle_venta`
--

LOCK TABLES `detalle_venta` WRITE;
/*!40000 ALTER TABLE `detalle_venta` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalle_venta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dias`
--

DROP TABLE IF EXISTS `dias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dias` (
  `id_Dia` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  PRIMARY KEY (`id_Dia`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dias`
--

LOCK TABLES `dias` WRITE;
/*!40000 ALTER TABLE `dias` DISABLE KEYS */;
INSERT INTO `dias` VALUES (1,'Domingo'),(2,'Lunes'),(3,'Martes'),(4,'Miercoles'),(5,'Jueves'),(6,'Viernes'),(7,'Sabado');
/*!40000 ALTER TABLE `dias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estatus`
--

DROP TABLE IF EXISTS `estatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estatus` (
  `id_Estatus` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(40) NOT NULL,
  PRIMARY KEY (`id_Estatus`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estatus`
--

LOCK TABLES `estatus` WRITE;
/*!40000 ALTER TABLE `estatus` DISABLE KEYS */;
INSERT INTO `estatus` VALUES (1,'Activo'),(2,'Inactivo'),(3,'Confirmada'),(4,'Cancelada'),(5,'Completada'),(6,'Surtido'),(7,'Pendiente');
/*!40000 ALTER TABLE `estatus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `generos`
--

DROP TABLE IF EXISTS `generos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `generos` (
  `id_Genero` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  PRIMARY KEY (`id_Genero`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `generos`
--

LOCK TABLES `generos` WRITE;
/*!40000 ALTER TABLE `generos` DISABLE KEYS */;
INSERT INTO `generos` VALUES (1,'Femenino'),(2,'Masculino'),(3,'No aplica');
/*!40000 ALTER TABLE `generos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `horarios`
--

DROP TABLE IF EXISTS `horarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `horarios` (
  `id_Horario` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Dia` int unsigned NOT NULL,
  `id_Medico` int unsigned NOT NULL,
  `hora_Entrada` time NOT NULL,
  `hora_Salida` time NOT NULL,
  PRIMARY KEY (`id_Horario`),
  KEY `fk_horarios_dia` (`id_Dia`),
  KEY `fk_horarios_medico` (`id_Medico`),
  CONSTRAINT `fk_horarios_dia` FOREIGN KEY (`id_Dia`) REFERENCES `dias` (`id_Dia`),
  CONSTRAINT `fk_horarios_medico` FOREIGN KEY (`id_Medico`) REFERENCES `medicos` (`id_Medico`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `horarios`
--

LOCK TABLES `horarios` WRITE;
/*!40000 ALTER TABLE `horarios` DISABLE KEYS */;
INSERT INTO `horarios` VALUES (1,3,3,'09:00:00','13:00:00'),(6,2,3,'09:00:00','13:00:00'),(10,4,3,'09:00:00','13:00:00'),(11,6,3,'09:00:00','13:00:00'),(12,5,3,'09:00:00','13:00:00');
/*!40000 ALTER TABLE `horarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicamentos`
--

DROP TABLE IF EXISTS `medicamentos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicamentos` (
  `id_Medicamento` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Estatus` int unsigned NOT NULL,
  `nombre_Medicamento` varchar(45) NOT NULL,
  `descripcion_Medicamento` varchar(255) NOT NULL,
  `precio_Medicamento` decimal(10,2) NOT NULL,
  `stock_Medicamento` int unsigned NOT NULL,
  `concentracion_Valor` decimal(10,2) NOT NULL,
  `concentracion_Unidad` varchar(10) NOT NULL,
  PRIMARY KEY (`id_Medicamento`),
  KEY `fk_medicamentos_estatus` (`id_Estatus`),
  CONSTRAINT `fk_medicamentos_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicamentos`
--

LOCK TABLES `medicamentos` WRITE;
/*!40000 ALTER TABLE `medicamentos` DISABLE KEYS */;
INSERT INTO `medicamentos` VALUES (1,1,'Paracetamol ','Para dolor',50.00,0,500.00,'mg'),(2,1,'Ibuprofeno','Para dolor',40.00,0,500.00,'mg'),(3,1,'Paracetamol','Para dolor',50.00,0,250.00,'mg'),(4,1,'Loradatadina','Para alergias',30.00,0,75.00,'mg'),(5,1,'Amoxicilina','Para infeccion',60.00,0,400.00,'mg'),(6,1,'Bromuro de Pinaverio','Para indigestion',50.00,0,25.00,'mg'),(7,1,'Levocetiricina','Para alergia',30.00,0,500.00,'mg'),(8,1,'Atorvastatina','Para corazon',500.00,0,600.00,'mg'),(9,1,'Cetiricina','Para infección',60.00,0,600.00,'mg');
/*!40000 ALTER TABLE `medicamentos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicos`
--

DROP TABLE IF EXISTS `medicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicos` (
  `id_Medico` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Usuario` int unsigned NOT NULL,
  `cedula_Profesional` varchar(30) NOT NULL,
  `especialidad` varchar(45) NOT NULL,
  PRIMARY KEY (`id_Medico`),
  KEY `fk_medicos_usuario` (`id_Usuario`),
  CONSTRAINT `fk_medicos_usuario` FOREIGN KEY (`id_Usuario`) REFERENCES `usuarios` (`id_Usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicos`
--

LOCK TABLES `medicos` WRITE;
/*!40000 ALTER TABLE `medicos` DISABLE KEYS */;
INSERT INTO `medicos` VALUES (3,14,'24567898','Medico General'),(9,18,'12345678','Médico general');
/*!40000 ALTER TABLE `medicos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `metodos_pago`
--

DROP TABLE IF EXISTS `metodos_pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `metodos_pago` (
  `id_Metodo` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  PRIMARY KEY (`id_Metodo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `metodos_pago`
--

LOCK TABLES `metodos_pago` WRITE;
/*!40000 ALTER TABLE `metodos_pago` DISABLE KEYS */;
INSERT INTO `metodos_pago` VALUES (1,'Tarjeta'),(2,'Efectivo');
/*!40000 ALTER TABLE `metodos_pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pacientes`
--

DROP TABLE IF EXISTS `pacientes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pacientes` (
  `id_Paciente` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Usuario` int unsigned NOT NULL,
  PRIMARY KEY (`id_Paciente`),
  KEY `fk_pacientes_usuario` (`id_Usuario`),
  CONSTRAINT `fk_pacientes_usuario` FOREIGN KEY (`id_Usuario`) REFERENCES `usuarios` (`id_Usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pacientes`
--

LOCK TABLES `pacientes` WRITE;
/*!40000 ALTER TABLE `pacientes` DISABLE KEYS */;
INSERT INTO `pacientes` VALUES (3,12),(2,13),(6,20),(7,21);
/*!40000 ALTER TABLE `pacientes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permisos`
--

DROP TABLE IF EXISTS `permisos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permisos` (
  `id_Permiso` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id_Permiso`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permisos`
--

LOCK TABLES `permisos` WRITE;
/*!40000 ALTER TABLE `permisos` DISABLE KEYS */;
INSERT INTO `permisos` VALUES (1,'CRUD de ventas'),(2,'CRUD de empleados'),(3,'CRUD de citas'),(4,'CRUD de roles'),(5,'CRUD de medicamentos'),(6,'CRUD de compras'),(7,'Reportes'),(8,'Registrar consulta'),(9,'Horario de medicos'),(10,'Editar perfil');
/*!40000 ALTER TABLE `permisos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permisos_has_roles`
--

DROP TABLE IF EXISTS `permisos_has_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permisos_has_roles` (
  `id_Rol` int unsigned NOT NULL,
  `id_Permiso` int unsigned NOT NULL,
  PRIMARY KEY (`id_Rol`,`id_Permiso`),
  KEY `fk_phr_permiso` (`id_Permiso`),
  CONSTRAINT `fk_phr_permiso` FOREIGN KEY (`id_Permiso`) REFERENCES `permisos` (`id_Permiso`),
  CONSTRAINT `fk_phr_rol` FOREIGN KEY (`id_Rol`) REFERENCES `roles` (`id_Rol`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permisos_has_roles`
--

LOCK TABLES `permisos_has_roles` WRITE;
/*!40000 ALTER TABLE `permisos_has_roles` DISABLE KEYS */;
INSERT INTO `permisos_has_roles` VALUES (1,1),(6,1),(1,2),(1,3),(2,3),(3,3),(6,3),(1,4),(1,5),(1,6),(6,6),(1,7),(2,7),(6,7),(2,8),(1,9),(1,10),(3,10);
/*!40000 ALTER TABLE `permisos_has_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proveedores`
--

DROP TABLE IF EXISTS `proveedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedores` (
  `id_Proveedor` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Estatus` int unsigned NOT NULL,
  `nombre_Proveedor` varchar(60) NOT NULL,
  `telefono_Proveedor` varchar(15) NOT NULL,
  `direccion_Proveedor` varchar(80) NOT NULL,
  PRIMARY KEY (`id_Proveedor`),
  KEY `fk_proveedores_estatus` (`id_Estatus`),
  CONSTRAINT `fk_proveedores_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proveedores`
--

LOCK TABLES `proveedores` WRITE;
/*!40000 ALTER TABLE `proveedores` DISABLE KEYS */;
INSERT INTO `proveedores` VALUES (1,1,'CGO','838432532','Azura'),(2,1,'Umbrella','423642533','Raccoon City');
/*!40000 ALTER TABLE `proveedores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id_Rol` int unsigned NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  PRIMARY KEY (`id_Rol`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Administrador'),(2,'Médico'),(3,'Paciente'),(6,'Asistente Farmacia');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id_Usuario` int unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(50) UNIQUE NULL,
  `id_Estatus` int unsigned NOT NULL,
  `id_Genero` int unsigned NOT NULL,
  `id_Rol` int unsigned NOT NULL,
  `nombre_Usuario` varchar(45) NOT NULL,
  `apellido_P` varchar(45) NOT NULL,
  `apellido_M` varchar(45) DEFAULT NULL,
  `email_Usuario` varchar(45) NOT NULL,
  `password_Usuario` varchar(255) NOT NULL,
  `telefono` varchar(15) NOT NULL,
  `fecha_Nacimiento` date NOT NULL,
  PRIMARY KEY (`id_Usuario`),
  KEY `fk_usuarios_estatus` (`id_Estatus`),
  KEY `fk_usuarios_genero` (`id_Genero`),
  KEY `fk_usuarios_rol` (`id_Rol`),
  CONSTRAINT `fk_usuarios_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`),
  CONSTRAINT `fk_usuarios_genero` FOREIGN KEY (`id_Genero`) REFERENCES `generos` (`id_Genero`),
  CONSTRAINT `fk_usuarios_rol` FOREIGN KEY (`id_Rol`) REFERENCES `roles` (`id_Rol`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` (id_Usuario, id_Estatus, id_Genero, id_Rol, nombre_Usuario, apellido_P, apellido_M, email_Usuario, password_Usuario, telefono, fecha_Nacimiento) VALUES (1,1,2,1,'Flavio Rafael','Aguilar','Moreno','flavio.flax1234@gmail.com','$2a$11$Z1OjvVfOXe681N4TawGSUes/RaanfdUbY9k.H3hVXB69vJ7KOF2Te','8445442696','2005-11-30');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ventas`
--

DROP TABLE IF EXISTS `ventas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ventas` (
  `id_Venta` int unsigned NOT NULL AUTO_INCREMENT,
  `id_Consulta` int unsigned DEFAULT NULL,
  `id_Estatus` int unsigned NOT NULL,
  `id_Metodo` int unsigned NOT NULL,
  `id_Usuario` int unsigned NOT NULL,
  `fecha_Venta` date NOT NULL,
  `hora_Venta` time DEFAULT NULL,
  `total_Venta` decimal(10,2) NOT NULL,
  `nombre_Cliente` varchar(70) DEFAULT NULL,
  PRIMARY KEY (`id_Venta`),
  KEY `fk_ventas_consulta` (`id_Consulta`),
  KEY `fk_ventas_estatus` (`id_Estatus`),
  KEY `fk_ventas_metodo` (`id_Metodo`),
  KEY `fk_ventas_usuario` (`id_Usuario`),
  CONSTRAINT `fk_ventas_consulta` FOREIGN KEY (`id_Consulta`) REFERENCES `consultas` (`id_Consulta`),
  CONSTRAINT `fk_ventas_estatus` FOREIGN KEY (`id_Estatus`) REFERENCES `estatus` (`id_Estatus`),
  CONSTRAINT `fk_ventas_metodo` FOREIGN KEY (`id_Metodo`) REFERENCES `metodos_pago` (`id_Metodo`),
  CONSTRAINT `fk_ventas_usuario` FOREIGN KEY (`id_Usuario`) REFERENCES `usuarios` (`id_Usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ventas`
--

LOCK TABLES `ventas` WRITE;
/*!40000 ALTER TABLE `ventas` DISABLE KEYS */;
/*!40000 ALTER TABLE `ventas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-25 18:15:10

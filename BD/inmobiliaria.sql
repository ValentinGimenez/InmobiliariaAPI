-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Sep 26, 2025 at 10:52 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--
CREATE DATABASE IF NOT EXISTS `inmobiliaria` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `inmobiliaria`;

-- --------------------------------------------------------

--
-- Table structure for table `auditoria`
--

DROP TABLE IF EXISTS `auditoria`;
CREATE TABLE `auditoria` (
  `id_auditoria` int(11) NOT NULL,
  `tipo` enum('Contrato','Pago','Inmueble','Usuario') NOT NULL,
  `id_registro_afectado` int(11) NOT NULL,
  `accion` enum('Crear','Actualizar','Finalizar','Anular','Recibir') NOT NULL,
  `usuario` varchar(100) NOT NULL,
  `fecha_hora` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `auditoria`
--

INSERT INTO `auditoria` (`id_auditoria`, `tipo`, `id_registro_afectado`, `accion`, `usuario`, `fecha_hora`) VALUES
(1, 'Contrato', 1, 'Crear', 'admin@gmail.com', '2025-09-26 17:46:29'),
(2, 'Contrato', 2, 'Crear', 'admin@gmail.com', '2025-09-26 17:47:05'),
(3, 'Pago', 1, 'Recibir', 'admin@gmail.com', '2025-09-26 17:47:14');

-- --------------------------------------------------------

--
-- Table structure for table `contrato`
--

DROP TABLE IF EXISTS `contrato`;
CREATE TABLE `contrato` (
  `id` int(11) NOT NULL,
  `id_inquilino` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `estado` int(11) DEFAULT 1,
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `multa` decimal(10,2) DEFAULT NULL,
  `monto_mensual` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contrato`
--

INSERT INTO `contrato` (`id`, `id_inquilino`, `id_inmueble`, `fecha_inicio`, `fecha_fin`, `estado`, `fecha_terminacion_anticipada`, `multa`, `monto_mensual`) VALUES
(1, 1, 2, '2025-09-26', '2025-12-26', 1, NULL, NULL, 678123.00),
(2, 1, 1, '2025-10-31', '2026-03-31', 1, NULL, NULL, 560000.00);

--
-- Triggers `contrato`
--
DROP TRIGGER IF EXISTS `generar_pagos_contrato`;
DELIMITER $$
CREATE TRIGGER `generar_pagos_contrato` AFTER INSERT ON `contrato` FOR EACH ROW BEGIN
    DECLARE fecha_iteracion DATE;
    DECLARE contador INT DEFAULT 1;
    DECLARE total_meses INT;

    SET total_meses = PERIOD_DIFF(DATE_FORMAT(NEW.fecha_fin, '%Y%m'), DATE_FORMAT(NEW.fecha_inicio, '%Y%m'));

    IF DAY(NEW.fecha_fin) < DAY(NEW.fecha_inicio) THEN
        SET total_meses = total_meses - 1;
    END IF;

    IF total_meses < 1 THEN
         SET total_meses = 1; 
    END IF;

    SET fecha_iteracion = NEW.fecha_inicio;


    WHILE contador <= total_meses DO
        INSERT INTO pago (id_contrato, nro_pago, estado, concepto)
        VALUES (
            NEW.id, 
            contador, 
            'pendiente',

            CONCAT('Pago ', contador, ' | Mes de ', DATE_FORMAT(fecha_iteracion, '%M %Y'))
        );

        SET fecha_iteracion = DATE_ADD(fecha_iteracion, INTERVAL 1 MONTH);
        SET contador = contador + 1;
    END WHILE;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `inmueble`
--

DROP TABLE IF EXISTS `inmueble`;
CREATE TABLE `inmueble` (
  `id` int(11) NOT NULL,
  `id_propietario` int(11) NOT NULL,
  `id_tipo` int(11) NOT NULL,
  `direccion` varchar(250) NOT NULL,
  `uso` enum('Residencial','Comercial') NOT NULL,
  `ambientes` int(11) DEFAULT NULL,
  `eje_x` varchar(50) DEFAULT NULL,
  `eje_y` varchar(50) DEFAULT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` enum('Disponible','Suspendido','Alquilado') NOT NULL DEFAULT 'Disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inmueble`
--

INSERT INTO `inmueble` (`id`, `id_propietario`, `id_tipo`, `direccion`, `uso`, `ambientes`, `eje_x`, `eje_y`, `precio`, `estado`) VALUES
(1, 1, 1, 'Lavalle 1290', 'Residencial', 7, '2000', '3000', 560000.00, 'Alquilado'),
(2, 2, 2, 'Mitre 983', 'Comercial', 2, '3000', '1000', 678123.00, 'Alquilado');

-- --------------------------------------------------------

--
-- Table structure for table `inquilino`
--

DROP TABLE IF EXISTS `inquilino`;
CREATE TABLE `inquilino` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `estado` int(11) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inquilino`
--

INSERT INTO `inquilino` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`, `estado`) VALUES
(1, 'FLAVIO', 'MENDOZA', '00000003', 'flavio@gmail.com', '266400003', 1),
(2, 'CANDELA', 'ESCUDERO', '00000004', 'candela@gmail.com', '2664000005', 1);

-- --------------------------------------------------------

--
-- Table structure for table `pago`
--

DROP TABLE IF EXISTS `pago`;
CREATE TABLE `pago` (
  `id` int(11) NOT NULL,
  `id_contrato` int(11) NOT NULL,
  `nro_pago` int(11) NOT NULL,
  `fecha_pago` date DEFAULT NULL,
  `estado` enum('pendiente','recibido','anulado') NOT NULL DEFAULT 'pendiente',
  `concepto` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pago`
--

INSERT INTO `pago` (`id`, `id_contrato`, `nro_pago`, `fecha_pago`, `estado`, `concepto`) VALUES
(1, 1, 1, '2025-09-26', 'recibido', 'Pago 1 | Mes de September 2025'),
(2, 1, 2, NULL, 'pendiente', 'Pago 2 | Mes de October 2025'),
(3, 1, 3, NULL, 'pendiente', 'Pago 3 | Mes de November 2025'),
(4, 2, 1, NULL, 'pendiente', 'Pago 1 | Mes de October 2025'),
(5, 2, 2, NULL, 'pendiente', 'Pago 2 | Mes de November 2025'),
(6, 2, 3, NULL, 'pendiente', 'Pago 3 | Mes de December 2025'),
(7, 2, 4, NULL, 'pendiente', 'Pago 4 | Mes de January 2026'),
(8, 2, 5, NULL, 'pendiente', 'Pago 5 | Mes de February 2026');

-- --------------------------------------------------------

--
-- Table structure for table `propietario`
--

DROP TABLE IF EXISTS `propietario`;
CREATE TABLE `propietario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `estado` int(11) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietario`
--

INSERT INTO `propietario` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`, `estado`) VALUES
(1, 'VALENTIN', 'GIMENEZ', '43764888', 'valentingimenez1909@gmail.com', '2664326662', 1),
(2, 'ROBERTA', 'VALLEJOS', '43690464', 'roberta.vallejos@gmail.com', '2664970148', 1);

-- --------------------------------------------------------

--
-- Table structure for table `tipo_inmueble`
--

DROP TABLE IF EXISTS `tipo_inmueble`;
CREATE TABLE `tipo_inmueble` (
  `id` int(11) NOT NULL,
  `tipo` varchar(100) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tipo_inmueble`
--

INSERT INTO `tipo_inmueble` (`id`, `tipo`, `estado`) VALUES
(1, 'Casa', 1),
(2, 'Burger King', 1),
(3, 'Departamento', 1);

-- --------------------------------------------------------

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rol` enum('Admin','Empleado') NOT NULL,
  `estado` int(11) NOT NULL DEFAULT 1,
  `avatar` varchar(2550) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usuario`
--

INSERT INTO `usuario` (`id`, `nombre`, `apellido`, `dni`, `email`, `password`, `rol`, `estado`, `avatar`) VALUES
(1, 'ADMIN', '1', '00000001', 'admin@gmail.com', '123', 'Admin', 1, 'https://ui-avatars.com/api/?name=ADMIN%201&background=343a40&color=fff&rounded=true&size=128'),
(2, 'EMPLEADO', '1', '00000002', 'empleado@gmail.com', '123', 'Empleado', 1, 'https://ui-avatars.com/api/?name=EMPLEADO%201&background=343a40&color=fff&rounded=true&size=128');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `auditoria`
--
ALTER TABLE `auditoria`
  ADD PRIMARY KEY (`id_auditoria`);

--
-- Indexes for table `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_inquilino` (`id_inquilino`),
  ADD KEY `id_inmueble` (`id_inmueble`);

--
-- Indexes for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_propietario` (`id_propietario`),
  ADD KEY `id_tipo` (`id_tipo`);

--
-- Indexes for table `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_contrato` (`id_contrato`);

--
-- Indexes for table `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `auditoria`
--
ALTER TABLE `auditoria`
  MODIFY `id_auditoria` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id`);

--
-- Constraints for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id`),
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`id_tipo`) REFERENCES `tipo_inmueble` (`id`);

--
-- Constraints for table `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: brwukkns2fwpmgwkz0ws-mysql.services.clever-cloud.com:3306
-- Generation Time: Nov 10, 2025 at 06:04 AM
-- Server version: 8.4.2-2
-- PHP Version: 8.2.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `brwukkns2fwpmgwkz0ws`
--

-- --------------------------------------------------------

--
-- Table structure for table `contrato`
--

CREATE TABLE `contrato` (
  `id` int NOT NULL,
  `id_inquilino` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `estado` int DEFAULT '1',
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `multa` decimal(10,2) DEFAULT NULL,
  `monto_mensual` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Triggers `contrato`
--
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

CREATE TABLE `inmueble` (
  `id` int NOT NULL,
  `id_propietario` int NOT NULL,
  `tipo` int DEFAULT NULL,
  `direccion` varchar(250) COLLATE utf8mb4_general_ci NOT NULL,
  `uso` int DEFAULT NULL,
  `ambientes` int DEFAULT NULL,
  `eje_x` double DEFAULT NULL,
  `eje_y` double DEFAULT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` int DEFAULT NULL,
  `imagen` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `superficie` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `inquilino`
--

CREATE TABLE `inquilino` (
  `id` int NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `telefono` varchar(20) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `estado` int DEFAULT '1',
  `imagen` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `pago`
--

CREATE TABLE `pago` (
  `id` int NOT NULL,
  `id_contrato` int NOT NULL,
  `nro_pago` int NOT NULL,
  `fecha_pago` date DEFAULT NULL,
  `estado` int DEFAULT NULL,
  `concepto` varchar(255) COLLATE utf8mb4_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `propietario`
--

CREATE TABLE `propietario` (
  `id` int NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `telefono` varchar(20) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `estado` int DEFAULT '1',
  `clave` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietario`
--

INSERT INTO `propietario` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`, `estado`, `clave`) VALUES
(1, 'Valentino', 'Gimeno', '43764123', 'test@gmail.com', '2664123456', 1, '$2a$11$0RzWOouBYlU1y8i.0m/w9eAcIiThterupklrmSjXasx9Ip1uN.zIe'),
(2, 'Miguel', 'Mercado', '12345678', 'lmercado@gmail.com', '987654321', 1, '$2a$11$jsughDiyvebSNgTptEifROdp82fraXpy9l.vsp2XSjs9YlzKLgz2q'),
(3, 'Mariano', 'Luzza', '23456789', 'mluzza@gmail.com', '987654321', 1, '$2a$12$VkpzowGBXHnJm50qSSKrheAJvo5dc645YRWh7wpge58V6fHS67HtO'),
(4, 'Pablo', 'Poder', '34567890', 'ppoder@gmail.com', '555123456', 1, '$2a$12$VkpzowGBXHnJm50qSSKrheAJvo5dc645YRWh7wpge58V6fHS67HtO');

--
-- Indexes for dumped tables
--

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
  ADD KEY `id_propietario` (`id_propietario`);

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
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

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
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id`);

--
-- Constraints for table `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

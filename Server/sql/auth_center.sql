-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        5.7.15-log - MySQL Community Server (GPL)
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  9.4.0.5138
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- 导出 auth_center_db 的数据库结构
CREATE DATABASE IF NOT EXISTS `auth_center_db` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `auth_center_db`;

-- 导出  表 auth_center_db.uinfo 结构
CREATE TABLE IF NOT EXISTS `uinfo` (
  `uid` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT '玩家唯一UID号',
  `unick` varchar(32) NOT NULL DEFAULT '' COMMENT '玩家昵称',
  `usex` int(8) NOT NULL DEFAULT '0' COMMENT '玩家性别，0男1女',
  `uface` int(8) NOT NULL DEFAULT '0' COMMENT '系统默认的头像',
  `uname` varchar(32) NOT NULL DEFAULT '' COMMENT '玩家账号名称',
  `upwd` varchar(32) NOT NULL DEFAULT '' COMMENT '玩家账号密码',
  `phone` varchar(16) NOT NULL DEFAULT '' COMMENT '玩家电话',
  `email` varchar(64) NOT NULL DEFAULT '' COMMENT '玩家邮箱',
  `address` varchar(64) NOT NULL DEFAULT '' COMMENT '玩家地址',
  `uvip` int(8) NOT NULL DEFAULT '0' COMMENT '玩家vip等级',
  `vip_end_time` int(32) NOT NULL DEFAULT '0' COMMENT 'vip到期时间戳',
  `is_guest` int(8) NOT NULL DEFAULT '0' COMMENT '是否为游客账号',
  `guest_key` varchar(64) NOT NULL DEFAULT '0' COMMENT '游客账号唯一key',
  `status` int(8) NOT NULL DEFAULT '0',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COMMENT='存放玩家信息';

-- 数据导出被取消选择。
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

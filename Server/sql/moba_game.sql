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


-- 导出 moba_game 的数据库结构
CREATE DATABASE IF NOT EXISTS `moba_game` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `moba_game`;

-- 导出  表 moba_game.ugame 结构
CREATE TABLE IF NOT EXISTS `ugame` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '全局唯一的ID',
  `uid` int(11) NOT NULL DEFAULT '0' COMMENT '用户唯一ID号',
  `ucoin_1` int(11) NOT NULL DEFAULT '0' COMMENT '货币1',
  `ucoin_2` int(11) NOT NULL DEFAULT '0',
  `ucoin_3` int(11) NOT NULL DEFAULT '0',
  `uvip` int(11) NOT NULL DEFAULT '0',
  `uvip_endtime` int(11) NOT NULL DEFAULT '0',
  `uexp` int(11) NOT NULL DEFAULT '0' COMMENT '用户经验值',
  `ustatus` int(11) NOT NULL DEFAULT '0' COMMENT '账号状态，0正常',
  `uitem_1` int(11) NOT NULL DEFAULT '0' COMMENT '道具1',
  `uitem_2` int(11) NOT NULL DEFAULT '0' COMMENT '道具2',
  `uitem_3` int(11) NOT NULL DEFAULT '0' COMMENT '道具3',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='存放我们玩家在Moba游戏中的游戏数据\r\n金币，货币，道具，vip等级，账号状态，玩家经验\r\nuid为自增长的唯一ID';

-- 数据导出被取消选择。
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

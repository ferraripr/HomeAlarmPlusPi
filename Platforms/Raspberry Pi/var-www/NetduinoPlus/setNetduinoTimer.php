<?php
//USER PERSONAL SETTINGS
$NETDUINO_PLUS_PORT = 8085;
$system_time = exec("date +'%Y_%m_%d_%H_%M_%S'");
$link = "http://".$_SERVER['SERVER_NAME'].":" . $NETDUINO_PLUS_PORT . "/date-time_'.$system_time";
exec('curl \'. $link .'\'');
if(file_exists('../data/alerts.json'))
{
  unlink('../data/alerts.json');
}
?>

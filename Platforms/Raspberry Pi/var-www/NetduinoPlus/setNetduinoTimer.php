<?php
//USER PERSONAL SETTINGS
$NETDUINO_PLUS_PORT = 8085;
$system_time = exec("date +'%Y_%m_%d_%H_%M_%S'");
$link = "http://".$_SERVER['SERVER_NAME'].":" . $NETDUINO_PLUS_PORT . "/date-time_'.$system_time";
exec('curl \'. $link .'\'');
if(file_exists('../data/alerts.json'))
{
  echo '   attempting to delete alerts.json';
  unlink('../data/alerts.json');
}
else
{
  echo '   alerts.json does not exist!';
}

if(file_exists('../data/system-logs.json'))
{
  echo '   attempting to delete alerts.json';
  unlink('../data/system-logs.json');
}
else
{
  echo '   system-logs.json does not exist!';
}

?>

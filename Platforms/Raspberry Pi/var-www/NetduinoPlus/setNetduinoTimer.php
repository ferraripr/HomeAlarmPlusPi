<?php
$system_time = exec("date +'%Y_%m_%d_%H_%M_%S'");
exec('curl \'http://yourNetduinoPlusServer/date-time_'.$system_time.'\'');
?>

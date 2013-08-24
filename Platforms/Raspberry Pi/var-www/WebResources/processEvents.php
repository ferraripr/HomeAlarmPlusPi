<?php
//PUSHOVERSETTINGS
$APP_TOKEN = "Your Pushover App token";
$USER_KEY = "Your Pushover User Key";

$EVENT_DESCRIPTION = "";
$EVENT_TITLE = "";
$EVENT_TIME = "";

//TIME DISPLAY
  /*Time Display the PHP way*/
  /* http://www.php.net/manual/en/class.datetimezone.php */
  //East
  date_default_timezone_set('America/New_York');
  $current_time = strtoupper(date('d M Y h:i:s a T'));
  
  //format expected on web address: http://raspberrypi/WebResources/processEvents.php?alarm-description=this%20is%20a%20test&title=This%20is%20a%20title
  if (isset($_GET['title']))
  {
    $EVENT_TITLE = $_GET["title"];  
  }  
  if (isset($_GET['alarm-description']))
  {
    $EVENT_DESCRIPTION = trim($_GET["alarm-description"]);
    $TEMP_ALARM_DESCRIPTION = str_replace("_"," ",$EVENT_DESCRIPTION);
	
	//send notification to Pushover
	exec('curl -s   -F "token='.$APP_TOKEN.'"   -F "user='.$USER_KEY.'"   -F "message='.$TEMP_ALARM_DESCRIPTION.'."   -F "sound=climb"   -F "title='.$EVENT_TITLE.'"   https://api.pushover.net/1/messages.json');
  }  
  if (isset($_GET['Ntime']))
  {
    $EVENT_TIME = $_GET["Ntime"];  
  }
  if (isset($_GET['Alarm']))
  {
    $IS_ALARM = $_GET["Alarm"]; 
	if($IS_ALARM == "true")
	{	  	  	  
	  $content = split("_",$EVENT_DESCRIPTION);
	  $jrecord_arr = array('time' => $EVENT_TIME, 'zone' => trim($content[1]) ,'description' => trim($content[2]));
	  
	  $fp = fopen('../data/alerts.json', 'a+');
      fwrite($fp, json_encode($jrecord_arr)."\n");
      fclose($fp);
	}
	else
	{
	  if($EVENT_DESCRIPTION == "Time set from Raspberry Pi")
	  {
	     $output = shell_exec("php ../NetduinoPlus/setNetduinoTimer.php"); 
         $EVENT_TIME = date('m/d/Y h:i:s');
	  }

	  $content = split("_",$EVENT_DESCRIPTION);
	  //$jrecord_arr = array('time' => $EVENT_TIME,'description' => trim($content[1]));
	  $jrecord_arr = array('time' => $EVENT_TIME,'description' => trim($content[0]));
	  
	  $fp = fopen('../data/system-logs.json', 'a+');
      fwrite($fp, json_encode($jrecord_arr)."\n");
      fclose($fp);	  	   
	}
  }
  
  
    print <<< EOT
<!doctype html>
<html lang="en">
<head>
        <meta charset="utf-8" />
        <meta name="author"   content="Gilberto Garc&#237;a"/>
        <meta name="mod-date" content="08/24/2013"/>
	<meta name="viewport" content="width=device-width, initial-scale=1">
</head>
<body>
<p>Alarm Parse Debug window </p>
<p>Alarm Description = time: {$current_time}, alarm title: {$EVENT_TITLE}, alarm time {$EVENT_TIME} </p>
<p>Event Description = {$EVENT_DESCRIPTION} </p>
<p>Temp Alarm Description = {$TEMP_ALARM_DESCRIPTION} </p>
</body>
</html>
EOT;
?>

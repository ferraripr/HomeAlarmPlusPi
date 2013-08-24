<!-- http://phptutorial.info/?json-decode -->
<html>
<head>
<meta charset="utf-8" />
<meta name="author"   content="Gilberto Garc&#237;a">
<meta name="mod-date" content="07/05/2013">

<meta name="apple-mobile-web-app-title" content="Alarm Activity">
<meta name="apple-mobile-web-app-capable" content="yes">
<meta name="apple-mobile-web-app-status-bar-style" content="black">


<link rel="stylesheet" href="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css"/>		
<script src="http://code.jquery.com/jquery-1.10.0.min.js"></script>
		
		
<script src="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
</head>
<body class="ui-mobile-viewport ui-overlay-b">
        <div data-add-back-btn="true" style="padding-top: 44px; padding-bottom: 44px; min-height: 846px;" class="ui-page ui-body-b ui-page-header-fixed ui-page-footer-fixed" tabindex="0" data-url="alerts-page" data-role="page" id="AlarmPanel" data-theme="b" data-content-theme="d" data-overlay-theme="a" data-title="Alarm Activity">
            <div role="banner" class="ui-header ui-bar-a ui-header-fixed slidedown ui-fixed-hidden" data-role="header" data-position="fixed">
                <h3 aria-level="1" role="heading" class="ui-title">
                    ALERTS
                </h3>
            </div>

            <div role="main" class="ui-content ui-body-d" data-role="content">			
            <table class="gridtable">
            <tr><th><center>Time</center></th><th><center>Zone/Sensor</center></th><th><center>Description</center></th></tr>
            <br>
<?php
$DEBUG = 0;

$filename = "../data/alerts.json";
$fp = @fopen($filename, 'r'); 
$array = explode("\n", fread($fp, filesize($filename)));

if ($fp) 
{
   $ALERT_COUNT = count($array) -1;
   if($DEBUG)
   {
      echo $ALERT_COUNT;
   }
   if ($ALERT_COUNT >0)
   {
      // Add each line to an array
      for($i=0;$i<$ALERT_COUNT;++$i)
      {
         $json= $array[$i];
         $obj = json_decode($json);
		 echo "<td><center>" . $obj->{'time'} . "</center></td>";
		 echo "<td><center>" . $obj->{'zone'} . "</center></td>";
		 echo "<td><center>" . $obj->{'description'} . "</center></td>";
         echo "</tr>";
      }
   }
}
else
{
  echo "<tr><td></td><td><center>No Alarms/Sensors to report</center></td><td></td></tr>";
}



?>
</table>
<p id="c_zone"></p>
</div>
</div>
</body>
</html>
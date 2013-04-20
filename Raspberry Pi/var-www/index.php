<?php
//USER PERSONAL SETTINGS
$PushingBoxDeviceID = "Your PushingBox device ID";
$NETDUINO_PLUS_PORT = 8085;
$MOBILE = "0";
$WUNDERGROUND_KEY_ID = "Your Wunderground Key ID";
$ZIP_CODE = "Your Zip code";

$current_time = exec("date +'%d %b %Y %r %Z'");

if (isset($_GET['main-page']))
{
  if (isset($_GET['deviceID']))
  {
     $PushingBoxDeviceID = $_GET["deviceID"];
  }
  
  $MOBILE = isset($_GET['mobile'])? "1": "0";

  //send notification to PushingBox
  exec('curl \'http://api.pushingbox.com/pushingbox?devid='.$PushingBoxDeviceID.'\'');
  
  $output = shell_exec('ls -la');
  print <<< EOT
<!doctype html>
<html lang="en">
<!-- original content from HomeAlarmPlus project details on  http://netduinoexperience.blogspot.com/ -->
<!--jQuery, linked from a CDN-->
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js" type="text/javascript"></script>

<!-- Weather data from Wunderground, http://www.wunderground.com/weather/api/ -->
<script>
jQuery(document).ready(function($) {
  $.ajax({
  url : "http://api.wunderground.com/api/{$WUNDERGROUND_KEY_ID}/conditions/forecast/q/{$ZIP_CODE}.json",
  dataType : "jsonp",
  success : function(parsed_json) {

  var temp_f = parsed_json['current_observation']['temp_f'] + '&#8457;' ;
  var current_conditions = parsed_json['current_observation']['weather']  ;
  var location = parsed_json['current_observation']['display_location']['full'];
  var icon_url_current = parsed_json['current_observation']['icon_url'];

  var image_current = "<img src=\""+ icon_url_current +"\" width=\"40\" height=\"40\" >";
    
  $("#c_current_conditions").append("<br/><center>"+image_current+"<br/><b>" + current_conditions +"</b></center>");
  $("#c_location").append("<b>" + location +"</b>");
  $("#c_temperature").append("<br/><center><b><font size=\"6\">" + temp_f +"</font></b></center>");
  }
  });
});
</script>

<head>
<meta http-equiv="Content-Style-Type" content="text/css">
<meta charset="UTF-8">
<meta name="author"   content="Gilberto Garc&#237;a"/>
<meta name="mod-date" content="04/20/2013"/>

<!-- http://www.formsite.com/documentation/mobile-optimization.html -->
<?php if ($MOBILE ==1) : ?>
   <meta name="viewport" content="width=device-width, height=device-height, user-scalable=no" />
   <meta name="MobileOptimized" content="width" />
   <meta name="HandheldFriendly" content="true" />
   <meta name="apple-mobile-web-app-title" content="HomeAlarmPlus Pi" />
   <meta name="apple-mobile-web-app-capable" content="yes" />
   <meta name="apple-mobile-web-app-status-bar-style" content="black" />
<?php endif; ?>

<link rel="stylesheet" type="text/css" href="WebResources/header_style.css"></style>
<link rel="stylesheet" type="text/css" href="WebResources/table_style.css"></style>
<title>RASPBERRY PI Control Panel - Home</title>
</head>
<body>
		<h1>HomeAlarmPlus Pi</h1>	
        <p>System Time: <b>{$current_time}</b></p>
		<p id="c_location">Location: </p>
		<p>DEBUG: Value of Mobile is :{$MOBILE}</p>
		<br/>
<div><ul>
<!--
<li class="current"><a href="/" title='Home'>HOME</a></li>
-->
<li class="toplinks"><a href="http://{$_SERVER['SERVER_NAME']}:{$NETDUINO_PLUS_PORT}" target="_blank" title='Access alarm panel'>Alarm Panel [Netduino Plus]</a></li>
<li class="toplinks"><a href="/weather.html" target="_blank" title='weather'>Weather</a></li>
<li class="toplinks"><a href="/sysinfo/index.php" target="_blank" title='System Info'>System Info</a></li>
<li class="toplinks"><a href="/sysinfo_v2/index.php" target="_blank" title='System Info'>System Info v2</a></li>
<li class="toplinks"><a href="/mobile/main.html" target="_blank" title='Mobile'>Mobile version</a></li>

<li class="toplinks"><a href='/about.htm' title='Credits and contributors'>ABOUT</a> </li>
</ul></div>
        </br>
		<table class="gridtable" border="1" width="20%">
		        <tr>
                   <td id="c_current_conditions"><center>Currently</center><br/></td>
				   <td id="c_temperature"><center>Temperature</center></td>
				</tr> 
	   </table>
<br/><br/><br/><br/><br/><br/><br/>
<div style="border:1px solid #CCCCCC;">
<p><span class="note">Copyright &#169; 2012, 2013 Gilberto Garc&#237;a</span></p>
</div>
</body>
</html>
EOT;
}

else
{
   $useragent=$_SERVER['HTTP_USER_AGENT'];
   if(preg_match('/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i',$useragent)||preg_match('/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i',substr($useragent,0,4)))
   {
	   //header("Location: http://detectmobilebrowsers.com/");
	   header("Location: http://" . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'] . "?main-page=yes&deviceID=".$PushingBoxDeviceID ."&mobile=yes");
   }
   else
   {
       header("Location: http://" . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'] . "?main-page=yes&deviceID=".$PushingBoxDeviceID);
   }
   exit;
}
?>

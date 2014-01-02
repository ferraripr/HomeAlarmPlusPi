<?php
//USER PERSONAL SETTINGS
$RASPBERRYPI1_PORT = 8086;
$NETDUINO_PLUS_PORT = 8085;
?>
<!DOCTYPE HTML>
<html>
	<head>
	    <title>Control Panel - About</title>
       <meta name="author"   content="Gilberto García"/>
       <meta name="mod-date" content="01/02/2014"/>
       <!--jQuery, linked from a CDN-->
       <script src="http://code.jquery.com/jquery-1.9.1.js"></script>
       <script src="http://code.jquery.com/ui/1.10.2/jquery-ui.js"></script>
       <!--jQueryUI Theme -->
       <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.2/themes/redmond/jquery-ui.css" />
       <link rel="stylesheet" type="text/css" href="../WebResources/header_style.css"></style>
	</head>
	<body>
       <div class="ui-widget">
       <div class="ui-widget-header ui-corner-top">
		<h2>Alarm Activity - Monitor System #1</h2></div>
       <div class="ui-widget-content ui-corner-bottom">
	   <br>
       <?php
          $link2 = "http://".$_SERVER['SERVER_NAME']. ":" . $NETDUINO_PLUS_PORT . "/assy";
          $netduino_file = file_get_contents($link2);
          echo $netduino_file;
       ?>
       <br>
           Hardware: <a href="http://netduino.com/netduinoplus/specs.htm" target="_blank">Netduino Plus</a>
       <br>
       <br>
       <div>
           <ul>
           <li class="toplinks">
           <?php 
		      $link = "http://".$_SERVER['SERVER_NAME'].":" .$RASPBERRYPI1_PORT . "/references.htm";
		      echo "<a href= $link target='_blank' title='Credits and contributors'>References</a>"; 
		   ?>
		   </li>
           </ul>
        </div><br>
        <br>
        <?php 
		$link = "http://".$_SERVER['SERVER_NAME'].":" .$NETDUINO_PLUS_PORT;
		echo "<a href= $link>Back to main page...</a>"; 
		?>
        <br>
        <div style="border:1px solid #CCCCCC;">        <p><span class="note">Copyright &#169; 2014 Gilberto Garc&#237;a</span></p>        </div>	</div></div></body>
</html>
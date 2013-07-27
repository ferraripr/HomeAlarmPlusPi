<?php
//USER PERSONAL SETTINGS
$RASPBERRYPI1_PORT  = 8086;
$NETDUINO_PLUS_PORT = 8085;

//PUSHOVERSETTINGS
$APP_TOKEN = "Your Pushover App token";
$USER_KEY  = "Your Pushover User Key";
?>
<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8" />
        <meta name="author"   content="Gilberto Garc&#237;a"/>
        <meta name="mod-date" content="07/27/2013"/>
        <meta name="viewport" content="width=device-width, height=device-height, user-scalable=no" />
        <meta name="MobileOptimized" content="width" />
        <meta name="HandheldFriendly" content="true" />
        <meta name="apple-mobile-web-app-title" content="HomeAlarmPlus Pi Mobile" />
        <meta name="apple-mobile-web-app-capable" content="yes" />
        <meta name="apple-mobile-web-app-status-bar-style" content="black" />		
        <link rel="stylesheet" type="text/css" href="../WebResources/jquery_table_style_blue.css"></style>
		
        <!--http://jsfiddle.net/frankdenouter/Lp9P2/-->
        <link rel="stylesheet" href="../WebResources/weather_table.css" />
		
        <link rel="stylesheet" href="http://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link rel="stylesheet" href="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css">
		
	<link rel="stylesheet"  href="http://jquerymobile.com/demos/1.3.0-beta.1/docs/demos/_assets/css/jqm-demos.css">
		
        <script src="http://code.jquery.com/jquery-1.7.2.min.js"></script>
		
        <script>
            $(document).bind("mobileinit", function(){
	        $.mobile.defaultPageTransition = 'flow';
                $.mobile.defaultDialogTransition = 'slide';
                $.mobile.loadingMessageTextVisible = true;
                $.mobile.pageLoadErrorMessage = 'Page cannot be accessed this time!';				
            });
        </script>
		
        <script src="http://code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js"></script>
		
        <!-- Weather data from Wunderground, http://www.wunderground.com/weather/api/ -->
        <script src="../WebResources/wunderground_query.js"></script>
		
        <!-- http://jsfiddle.net/jerone/gsNzT/ -->
        <script src="../WebResources/jquery_animate_collapse.js"></script>
	
        <script>
            $(document).on('pagebeforecreate', '[data-role="page"]', function(){     
                setTimeout(function(){
                     $.mobile.loading('show');
    },1);    
});

            $(document).on('pageshow', '[data-role="page"]', function(){  
                setTimeout(function(){
                   $.mobile.loading('hide');
                },1000);	
});
        </script>


<script>
$(document).on("swipeleft", function(event, ui) {
$( "#nav-panel").panel("close", {display: 'push', position: 'right'} );
});
$(document).on("swiperight", function(event, ui) {
$( "#nav-panel").panel("open", {display: 'overlay', position: 'left'} );
});
</script>
	<style>
        #navilink {
    	   -webkit-border-radius: 0.1em /*{global-radii-buttons}*/;
           border-radius: 0.1em /*{global-radii-buttons}*/;                   
	   border-style: hidden;
        }
	</style>
		
    </head>
    <body>
              <?php
	         exec('curl -s   -F "token='.$APP_TOKEN.'"   -F "user='.$USER_KEY.'"   -F "message=Web Server Access from Pi Mobile."   -F "title=Web Trigger"   https://api.pushover.net/1/messages.json');
               ?>

        <!-- Home -->
        <div data-role="page" id="main-page" data-theme="b" data-content-theme="b" class="jqm-demos">
            <div data-theme="b" data-role="header" class="jqm-header" data-position="fixed" >
                <h3>
                    HomeAlarmPlus Pi
                </h3>
                <a id="navilink" href="#nav-panel" data-theme="b"   data-icon="bars" data-role="button" data-iconpos="notext" data-shadow="false" data-iconshadow="false" class="ui-icon-nodisc" >Menu</a>
            </div>
        <div data-role="content">
            <div class="content-primary">
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

        </div><!-- /content -->
	
		<br /><br /><br /><br />

        <div data-role="footer" class="footer-docs" data-theme="c" data-position="fixed">
		    <p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>
	    </div>
		
			<style>
				.nav-search .ui-btn-up-a {
					background-image:none;
					background-color:#333333;
				}
				.nav-search .ui-btn-inner {
					border-top: 1px solid #888;
					border-color: rgba(255, 255, 255, .1);
				}
            </style>

				<div data-role="panel" data-position="left"  data-display="reveal" data-dismissible="true" id="nav-panel" data-theme="a">
					<ul data-role="listview" data-theme="a" data-divider-theme="a" style="margin-top:-16px;" class="nav-search">
					    <li data-role="list-divider">Main Menu</li>
						<li data-icon="delete" style="background-color:#111;">
						    <a href="#" data-rel="close">Close menu</a>
                                                </li>
                                                <li data-filtertext="Settings">
                                                    <a href="#" rel="external">Settings</a>
                                                </li>						
                                                <li data-filtertext="Cameras">
                                                    <a href="#" rel="external">Cameras</a>
                                                </li>
						<li data-filtertext="weather data">
                                                    <a href="#weather" rel="external">Weather</a>
						</li>
						<li data-filtertext="Diagnostics">
						    <a href="diagnostics.php" rel="external" >Diagnostics</a>
						</li>						
						
						<div data-role="collapsible-set" data-corners="false" data-theme="a">
						<div data-role="collapsible" data-collapsed-icon="arrow-d" and data-expanded-icon="arrow-u" data-iconpos="right" data-collapsed="true">
						<h3>About</h3>
                                                <ul data-role="listview" data-theme="a" data-divider-theme="a">
                                                    <li data-filtertext="About Home Alarm Plus">								
                                                        <a href="#about-netduino" rel="external">HomeAlarmPlus</a>
                                                    </li>
                                                    <li data-filtertext="About Home Alarm Plus Pi">
                                                        <a href="#about-hapluspi" rel="external" >HomeAlarmPlus Pi</a>
                                                    </li>
                                                </ul>
						</div>
					</ul>
				</div><!-- /panel -->
        </div><!-- /main-page -->
		</div>
				
        <div data-role="page" id="weather"  data-theme="b" data-content-theme="b" class="jqm-demos">
            <div data-theme="b" data-role="header" class="jqm-header" data-position="fixed">
                <h3>
                    Weather
                </h3>
		                <?php 
                           $link = "http://".$_SERVER['SERVER_NAME'].":" . $RASPBERRYPI1_PORT . "/mobile/index.php";
                           echo "<a href= $link data-rel='back'  class='ui-btn-left ui-btn-back' data-icon='arrow-l' >Back</a>"; 
                        ?>
				<div data-role="content"><p id="c_location">Location: </p>
                <p id="c_alerts"><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>
                   <strong id="s_location">Alerts for </strong> 
				</p>
				</div>
            </div>			
            <div data-role="collapsible-set" data-content-theme="e">
               <div data-theme="c" data-role="collapsible" data-collapsed="false">
                  <h3>
                     Currently
                  </h3>
				  <table  border="0"  >
		             <tr>
                        <td id="c_current_conditions"></td>
				        <td id="c_temperature"><center>Temperature</center></td>
						<td id="c_current_precipitation"><center>Chance of<br>precipitation</center></td>
				    </tr> 
	              </table>
               </div>			
               <div data-theme="c" data-role="collapsible">
                  <h3>
                     3 day Forecast
                  </h3>
			      <table class="gridtable" border="0" width="100%">
			         <tr>
				        <td><center>Forecast</center></td>
			         </tr>				  
			         <tr>
				        <td id="c_forecast"></td>
			         </tr>
	              </table>
               </div>
               <div data-theme="c" data-role="collapsible" >
                  <h3>
                     10 day Forecast
                  </h3>
			      <table class="gridtable" border="0" width="100%">
			         <tr>
				        <td><center>Extended Forecast</center></td>
			         </tr>				  				  
			         <tr>
				        <td id="c_extforecast"></td>
			         </tr>
	              </table>
               </div>
           </div>
		   <div data-role="footer" class="footer-docs" data-theme="c" data-position="fixed">
		       <p class="jqm-version"></p>
		       <p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>
	       </div>		   
        </div><!-- /weather page -->
			
        <div data-role="page" id="about-hapluspi" data-theme="b" data-content-theme="b" class="jqm-demos">
            <div data-theme="b" data-role="header" data-position="fixed" >
                <h3>
                    About - HomeAlarmPlus Pi
                </h3>
                <?php 
                           $link = "http://".$_SERVER['SERVER_NAME'].":" . $RASPBERRYPI1_PORT . "/mobile/index.php";
                           echo "<a href= $link data-rel='back'  class='ui-btn-left ui-btn-back' data-icon='arrow-l' >Back</a>"; 
                ?>	
            </div>          
			<div data-role='content'>						
				<div data-role="content" data-content-theme="e">
				    <div class="content-primary">
				        <h2>HomeAlarmPlus Pi</h2>
				        <p>Programmed by Gilberto Garc&#237;a</p>
                                        <p>Weather data from <a href="http://www.wunderground.com/" rel="external">Wunderground.com.</a></p>
				        <p>For latest source code visit: <a href="https://github.com/ferraripr/HomeAlarmPlusPi" rel="external">Repository</a></p>
				    </div>
				</div>
            </div>
		   <div data-role="footer" class="footer-docs" data-theme="c" data-position="fixed">
		   <p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>
           </div>			
        </div><!-- /about raspberry pi page -->

        <div data-role="page" id="about-netduino" data-theme="b" data-content-theme="b" >
            <div data-theme="b" data-role="header"  data-position="fixed">
                <h3>
                    About - HomeAlarmPlus
                </h3>
		        <?php 
                           $link = "http://".$_SERVER['SERVER_NAME'].":" . $RASPBERRYPI1_PORT . "/mobile/index.php";
                           echo "<a href= $link data-rel='back'  class='ui-btn-left ui-btn-back' data-icon='arrow-l' >Back</a>"; 
                ?>
            </div>          
			<div data-role='content'>						
				    <div class="content-primary">
				        <h2>HomeAlarmPlus</h2>
				        <p>Programmed by Gilberto Garc&#237;a</p>						
                        <p>Weather data from <a href="http://www.wunderground.com/" rel='external'>Wunderground.com.</a></p>
				        <p>For latest source code visit: <a href="https://github.com/ferraripr/HomeAlarmPlusPi" rel='external'>Repository</a></p>
                        <p><b>Hardware:</b> <a href='http://netduino.com/netduinoplus/specs.htm' target='_blank'>Netduino Plus</a></p>
						<ul>
                          <li class="toplinks">
                          <?php 
                             $link = "http://".$_SERVER['SERVER_NAME'].":" . $RASPBERRYPI1_PORT . "/mobile/references-mobile.html";
                             echo "<a href= $link rel='external'  title='Credits and contributors'>References</a>"; 
                          ?>
						  </li>
                          <br>
                        </ul>
						<?php						   
                           try {
                                $link2 = "http://".$_SERVER['SERVER_NAME'].":" . $NETDUINO_PLUS_PORT . "/assy";
                                $netduino_file = file_get_contents($link2);
                                echo $netduino_file;
                           } 
                           catch (Exception $e) {
                                echo 'Page not available this time!';
                           }
						?>
				    </div>
            </div>
		   <div data-role="footer" class="footer-docs" data-theme="c" data-position="fixed">
		       <p>Copyright 2012, 2013 Gilberto Garc&#237;a</p>
	       </div>			
        </div><!-- /about netduino plus page -->		
		
		<br>
    </body>
</html>

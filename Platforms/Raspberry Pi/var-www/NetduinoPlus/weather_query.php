<?php
//USER PERSONAL SETTINGS
$NETDUINO_PLUS_PORT = 8085;

//WUNDERGROUND DATA
$Wunderground_Key_ID = "Your Wunderground ID";
$ZipCode = "Your Zip Code";

  $json_string = file_get_contents('http://api.wunderground.com/api/' .$Wunderground_Key_ID. '/geolookup/conditions/forecast/q/FL/' .$ZipCode. '.json');
  $parsed_json = json_decode($json_string);
  $location = $parsed_json->{'location'}->{'city'};
  
  //current temperature in fahrenheit
  $temp_fc = $parsed_json->{'current_observation'}->{'temp_f'};
  //current temperature in celcius
  //$temp_fc = $parsed_json->{'current_observation'}->{'temp_c'};
  
  $current_conditions = $parsed_json->{'current_observation'}->{'weather'};
  $current_conditions = str_replace(" ","%20",$current_conditions);

  //fahrenheit
  $high_temp = $parsed_json->forecast->simpleforecast->forecastday[0]->high->fahrenheit;
  $low_temp =  $parsed_json->forecast->simpleforecast->forecastday[0]->low->fahrenheit;
  
  //celsius
  //$high_temp = $parsed_json->forecast->simpleforecast->forecastday[0]->high->celsius;
  //$low_temp =  $parsed_json->forecast->simpleforecast->forecastday[0]->low->celsius;
  
  echo "Current temperature in ${location} is: ${temp_fc}\n";
  echo "<br>Current conditions : ${current_conditions}\n";
  echo "<br>High : ${high_temp}\n";
  echo "<br>Low  : ${low_temp}\n";

  exec("curl \"http://" .$_SERVER['SERVER_NAME'].":" . $NETDUINO_PLUS_PORT . "/weat_" . $temp_fc . "_" . $current_conditions . "_" . $high_temp . "_" . $low_temp ."\"");
?>
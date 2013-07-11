var Wunderground_Key_ID = "Your Wunderground ID";
var ZipCode = "Your Zip Code";
jQuery(document).ready(function($) {
  $.ajax({
  url : "http://api.wunderground.com/api/" + Wunderground_Key_ID + "/conditions/forecast10day/alerts/q/" + ZipCode + ".json",
  dataType : "jsonp",
  success : function(parsed_json) {

  var temp_f = parsed_json['current_observation']['temp_f'] + '&#8457;' ;
  var current_conditions = parsed_json['current_observation']['weather']  ;
  var location = parsed_json['current_observation']['display_location']['full'];
  var icon_url_current = parsed_json['current_observation']['icon_url'];

  var image_current = "<img src=\""+ icon_url_current +"\" width=\"40\" height=\"40\" >";
  var tablet_image_current = "<img src=\""+ icon_url_current +"\" width=\"60\" height=\"60\" alt=\"Weather\" style=\"float:middle\">";

  //Uncomment the following lines for more extensive and detailed forecast.
  //var forecast = parsed_json['forecast']['txt_forecast']['forecastday'];
  /*
  Index values:
  0 = Today
  1 = Tonight
  2 = Tomorrow
  */
  /*
  for (index in forecast)
  {
    var counter = parseInt(forecast[index]['period'])+1;
    var image = "<img src=\""+ forecast[index]['icon_url'] +"\" >";
    $("#c_forecast").append("<br/><b>[" + counter + "] " +  image + forecast[index]['title'] + ": "+ forecast[index]['fcttext'] + "</b>");
  }
  */

  //var forecast = parsed_json['forecast']['simpleforecast']['forecastday'];

  //interested in next day forecast
  //var index = 2;
  //$("#c_forecast").append("<b>" + forecast[index]['date']['weekday'] + " " + forecast[index]['conditions'] + ". High of " + forecast[index]['high']['fahrenheit'] + '&#8457;' + " and Low of " + forecast[index]['low']['fahrenheit'] + '&#8457;' + ".</b>");

  // uncomment loop for multiple days.
  /*
  for (index in forecast)
  {
	 $("#c_forecast").append("<br/><b>" + forecast[index]['date']['weekday'] + " " + forecast[index]['conditions'] + ". High of " + forecast[index]['high']['fahrenheit'] + '&#8457;' + " and Low of " + forecast[index]['low']['fahrenheit'] + '&#8457;' + ".</b>");
  }
  */

  var forecast = parsed_json['forecast']['simpleforecast']['forecastday'];
  for (index in forecast)
  {
    var counter = parseInt(forecast[index]['period']);
    var image = "<img src=\""+ forecast[index]['icon_url'] +"\" width=\"40\" height=\"40\" >";
    var highAndLows = "<b>" + forecast[index]['high']['fahrenheit'] +"</b>"+ "|" + forecast[index]['low']['fahrenheit'] + ' &#8457; ';

	//today
	if(counter == 1){	
	   $("#c_current_forecast").append("<b>" + forecast[index]['date']['weekday'] + " " + forecast[index]['conditions'] + ". High of " + forecast[index]['high']['fahrenheit'] + '&#8457;' + " and Low of " + forecast[index]['low']['fahrenheit'] + '&#8457;' + ".</b>");
	   $("#c_forecast").append("<br/><td align=\"left\"><b>[" + counter + "] " +  image + forecast[index]['date']['weekday']  + "</b> " + highAndLows + forecast[index]['conditions'] + ", chance of precipitation " + forecast[index]['pop'] + "%.</td>") ;
	   $("#c_current_precipitation").append("<center><b><font size=\"6\">" + forecast[index]['pop'] + "%</font></b></center>");
	   $("#c_extforecast").append("<br/><td align=\"left\"><b>[" + counter + "] " +  image + forecast[index]['date']['weekday']  + "</b> " + highAndLows + forecast[index]['conditions'] + ", chance of precipitation " + forecast[index]['pop'] + "%.</td>") ;
	}
	//3 day forecast
	else if (counter <=4){
	   $("#c_forecast").append("<br/><td align=\"left\"><b>[" + counter + "] " +  image + forecast[index]['date']['weekday']  + "</b> " + highAndLows + forecast[index]['conditions'] + ", chance of precipitation " + forecast[index]['pop'] + "%.</td>") ;
	   $("#c_extforecast").append("<br/><td align=\"left\"><b>[" + counter + "] " +  image + forecast[index]['date']['weekday']  + "</b> " + highAndLows + forecast[index]['conditions'] + ", chance of precipitation " + forecast[index]['pop'] + "%.</td>") ;
	}
	//extended forecast
	else{
	$("#c_extforecast").append("<br/><td align=\"left\"><b>[" + counter + "] " +  image + forecast[index]['date']['weekday']  + "</b> " + highAndLows + forecast[index]['conditions'] + ", chance of precipitation " + forecast[index]['pop'] + "%.</td>") ;
	}	
  }

  var alert = parsed_json['alerts'];
  if (parsed_json['alerts'].length !=0)
  {
     for (index1 in alert)
     {
        $("#c_alerts").append(alert[index1]['description'] + "<br/>");
     }
  }
  else
  {
     $("#c_alerts").append("None");
  }
      
  $("#c_current_conditions").append("<br/><center>"+image_current+"<br/><b>" + current_conditions +"</b></center>");
  $("#c_location").append("<b>" + location +"</b>");
  $("#c_temperature").append("<br/><center><b><font size=\"6\">" + temp_f +"</font></b></center>");
  $("#c_radar").append("<img src=\"http://api.wunderground.com/api/" + Wunderground_Key_ID + "/animatedradar/q/" + ZipCode + ".gif?newmaps=1&timelabel=1&timelabel.y=10&num=5&delay=50\" alt=\"radar\">");
  $("#s_location").append(location + ":");
  $("#s_weatherCurrentIcon").append(tablet_image_current);
  }
  });
});
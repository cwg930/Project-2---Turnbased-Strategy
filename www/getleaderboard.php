<?php

buildLeaderboardTable(getLeaderboardData());


function getLeaderboardData() {
	$TITLE_ID 	= '2B36';
	$url 				= 'https://' . $TITLE_ID . '.playfabapi.com/Server/GetLeaderboard';
	$maxResults = 100;
	$playfabAPIKey = '1ROBHII4AKJNC8P5NOIUK5QZ6AJ91QSXRDEEX87FQ8P4XHQZ9C';
	$statisticName = 'XP';
	
	// The data to send to the API
	$postData = array(
		'StatisticName'   => $statisticName,
		'MaxResultsCount' => $maxResults
	);

	// Setup cURL
	$ch = curl_init($url);
	curl_setopt_array($ch, array(
	    CURLOPT_POST => TRUE,
	    CURLOPT_RETURNTRANSFER => TRUE,
	    CURLOPT_HTTPHEADER => array(
	        'Content-Type: application/json',
					'X-SecretKey: '.$playfabAPIKey
	    ),
	    CURLOPT_POSTFIELDS => json_encode($postData)
	));

	//execute post
	$response = curl_exec($ch);

	// Check for errors
	if($response === FALSE){
	    die(curl_error($ch));
	}

	// Decode the response
	$responseData = json_decode($response, TRUE);
	$leaderboard  = $responseData['data']['Leaderboard'];

	//close connection
	curl_close($ch);
	
	return $leaderboard;
}

function buildLeaderboardTable($leaderboard) {
	$out = '';
	$out .= '<html>';
	$out .= '<head>';
	$out .= '  <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">';
	$out .= '  <link rel="stylesheet" type="text/css" href="style.css">';
	$out .= '  <title>Battle Knights Leaderboard</title>';
	$out .= '</head>';
	$out .= '<body>';
	$out .= '  <div align="center">';
	$out .= '    <a href="http://www.battleknights.co.nf/play.html"><button type="button" class="btn btn2 navbar-btn"">Play Battle Knights</button></a>';
	$out .= '    <a href="http://www.battleknights.freevar.com/getleaderboard.php""><button type="button"" class="btn btn2 navbar-btn">View Leaderboard</button></a>';
	$out .= '  </div>';
	$out .= '  <table class="table table-striped">';
	$out .= '    <thead><tr>';
	$out .= '      <th>Username</th>';
	$out .= '      <th>XP</th>';
	$out .= '      <th>Rank</th>';
	$out .= '    </tr></thead>';
	$out .= '    <tbody>';
	foreach ($leaderboard as $entry) {
		$out .= '     <tr>';
		$out .= '      <td>' . $entry['DisplayName'] . '</td>';
		$out .= '      <td>' . $entry['StatValue']   . '</td>';
		$out .= '      <td>' . ($entry['Position']+1)  . '</td>';
		$out .= '     </tr>';
	}
	$out .= '    </tbody>';
	$out .= '  </table>';
	$out .= '</body>';
	$out .= '</html>';
	echo $out;
}


?>
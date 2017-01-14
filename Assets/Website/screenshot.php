<?php
     if ((($_FILES["file"]["type"] == "image/png") || ($_FILES["file"]["type"] == "image/jpeg") || ($_FILES["file"]["type"] == "image/pjpeg")) && ($_FILES["file"]["size"] < 20000000000)) { if ($_FILES["file"]["error"] > 0) { echo "Return Code: " . $_FILES["file"]["error"] . "
     "; } else { echo "Upload: " . $_FILES["file"]["name"] . "
     "; echo "Type: " . $_FILES["file"]["type"] . "
     "; echo "Size: " . ($_FILES["file"]["size"] / 1024) . " Kb
     "; echo "Temp file: " . $_FILES["file"]["tmp_name"] . "
     ";
      
     while (file_exists($_FILES["file"]["name"]))
     {
     echo $_FILES["file"]["name"] . " already exists, changing name. ";
	 $_FILES["file"]["name"] = rand() . $_FILES["file"]["name"];
     }
	
     move_uploaded_file($_FILES["file"]["tmp_name"], "images/" . $_FILES["file"]["name"]);
     echo "Stored in: " . $_FILES["file"]["name"];
     
     }
     } else { echo "Invalid file"; }
     ?>
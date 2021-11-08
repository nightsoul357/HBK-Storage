R1="\"Url\": \"$HBKStorageUrl\""
sed -i "s+\"Url\": \".*\"+$R1+g" appsettings.json
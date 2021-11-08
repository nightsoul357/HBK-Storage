R1="\"Url\": \"$HBKStorageUrl\""
sed -i "s+\"Url\": \".*\"+$R1+g" "/usr/share/nginx/html/appsettings.json"
nginx -g 'daemon off;'
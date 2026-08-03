#!/bin/bash

if [[ "$(uname)" == "Darwin" ]]; then
    NC_FLAG="-w"
else
    NC_FLAG="-q"
fi

TIME=$(echo "get_time" | nc $NC_FLAG 1 localhost 4212 2>/dev/null | grep -o '[0-9]\+' | tail -n 1)
DURATION=$(echo "get_length" | nc $NC_FLAG 1 localhost 4212 2>/dev/null | grep -o '[0-9]\+' | tail -n 1 )
# Get URL and trim trailing spaces
URL=$(echo "status" | nc $NC_FLAG 1 localhost 4212 2>/dev/null | \
      grep -o 'new input: [^)]*' | sed 's/new input: //' | sed 's/[[:space:]]*$//')

if [ -n "$TIME" ]; then
    cat <<EOF
{
  "time": $TIME,
  "totalDuration": $DURATION,
  "url": "$(echo "$URL" | sed 's/"/\\"/g')",
  "status": "playing"
}
EOF
else
    echo '{"time": null, "totalDuration": null, "url": null, "status": "stopped"}'
fi
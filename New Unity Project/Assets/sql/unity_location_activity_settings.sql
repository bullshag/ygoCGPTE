SELECT activity_key,
       is_enabled
FROM location_activity_settings
WHERE location_id = @location_id
ORDER BY activity_key;

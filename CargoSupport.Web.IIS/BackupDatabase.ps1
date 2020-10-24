$date = Get - Date - Format "yyyy-MM-dd"
New - Item - ItemType Directory - Path "D:\DBBackups\$date"
& "D:\DBBackups\Tools\bin\mongodump.exe"--out D:\DBBackups\$date\ --db ICDB
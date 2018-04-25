# Look, Ruby is the language I know, ok?
require "json"

puts "\n"
puts "*" * 80
puts "\tWINNITRON LAUNCHER SETUP\t"
puts "*" * 80
puts "\n"

def config_userdata_location
  userdata_config = "Assets/Options/winnitron_userdata_path.json"
  userdata = JSON.load(File.read(userdata_config))
  datadir = userdata["userDataPath"]

  default_userdata_folder = "C:/WINNITRON_UserData"

  puts "The Winnitron User Data folder is currently configured to:"
  puts "\n\t#{datadir}"

  if !File.exists?(datadir)
    puts "\tWARNING This folder does not exist!"
  end
  puts "\n"

  puts "Enter:"
  puts " (k) keep using this folder"
  puts " (d) use default (#{default_userdata_folder})"
  puts " (n) enter a new path"
  puts " (q) quit now"

  print "> "
  choice = STDIN.gets.chomp.downcase
  new_user_folder = if choice == "q"
    exit()
  elsif choice == "d"
    default_userdata_folder
  elsif choice == "n"
    print " new path> "
    STDIN.gets.chomp
  else
    datadir
  end

  File.open(userdata_config, "w") do |file|
    contents = {
      "userDataPath" => new_user_folder
    }.to_json
    file.write contents
  end
end


config_userdata_location
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

  puts " (k) keep using this folder"
  puts " (d) use default (#{default_userdata_folder})"
  puts " (n) enter a new path"

  print "> "
  choice = STDIN.gets.chomp.downcase
  new_user_folder = if choice == "d"
    default_userdata_folder
  elsif choice == "n"
    print " new path> "
    STDIN.gets.chomp
  else
    datadir
  end

  File.open(userdata_config, "w") do |file|
    file.write JSON.pretty_generate({ "userDataPath" => new_user_folder })
  end

  new_user_folder
end

def set_api_key(user_folder)
  opts_file = "#{user_folder}/Options/winnitron_options.json"
  options = JSON.load(File.read(opts_file))

  puts "\n\n"
  puts "Your Winnitron is currently using this API key:"
  puts "\n\t#{options['sync']['apiKey']}"

  puts "\nIf you don't have an API key yet, register your arcade machine at network.winnitron.com"
  puts "\n"

  puts " (k) keep using this key"
  puts " (n) enter a new key"

  print "> "
  choice = STDIN.gets.chomp.downcase
  options['sync']['apiKey'] = if choice == "n"
    print " new key> "
    STDIN.gets.chomp
  else
    options['sync']['apiKey']
  end

  File.open(opts_file, "w") do |file|
    file.write JSON.pretty_generate(options)
  end
end

user_folder = config_userdata_location
set_api_key(user_folder)

puts "\n\n"
puts "All done! You can further configure your Winnitron by editing #{user_folder}/Options/winnitron_options.json"
puts "\n\n"
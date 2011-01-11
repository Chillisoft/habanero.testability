require 'rake'
require 'albacore'

#This is the default entry point for the script(running rake from the command line will look for a file called 'Rakfile.rb' and then look for the default task inside it
task :default => [:clean_up,:do_habanero,:do_smooth,:do_faces,:do_testability, :clean_up] #This may look  wierd but what we are constructing is a hash where the key is :defaulte and the value for that key is the list of tasks

task :do_habanero => [:clean_habanero,:checkout_habanero,:msdo_habanero] # these are the sub tasks that the do_habanero task shall run

task :do_smooth => [:checkout_smooth,:copy_dlls_to_smooth_lib,:clean_smooth,:msdo_smooth]

task :do_faces => [:checkout_faces,:copy_dlls_to_faces_lib,:clean_faces,:msdo_faces]

task :do_testability => [:copy_dlls_to_testability_lib,:clean_testability,:msdo_testability,:commit_lib]

#Global($) Constants(begin with capital letter in ruby) for easier configuration of the script

$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"# nunit-console-x86.exe is run to prevent the "Profiler connection not established" error from old Ncover versions
$Nunit_options = '/xml=nunit-result.xml /config=test.config'
   

#do_habanero tasks
task :clean_habanero do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Habanero/trunk/bin/'
end
exec :checkout_habanero do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero/trunk" "temp/Habanero/trunk/" --username chilli --password chilli --non-interactive) 
end

msbuild :msdo_habanero do |msb| #builds habanero with msbuild
    msb.targets :rebuild 
	msb.properties :configuration => :Debug
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/Habanero/trunk/source/Habanero.sln"
  end
  
  
#do_smooth tasks
  task :copy_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'temp/Habanero Community/SmoothHabanero/trunk/lib'
end

  task :clean_smooth do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Habanero Community/SmoothHabanero/trunk/bin'
end

exec :checkout_smooth do |cmd| #command to check out smooth source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero Community/SmoothHabanero/trunk/" "temp/Habanero Community/SmoothHabanero/trunk/" --username chilli --password chilli --non-interactive)
	# %q(...) is used to encase the parameters in a quote, necessary  because of the space in 'Habanero Community'
end

msbuild :msdo_smooth do |msb| #builds smooth with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/Habanero Community/SmoothHabanero/trunk/source/SmoothHabanero_2010.sln"
  end
  
  
#do_faces tasks
  task :copy_dlls_to_faces_lib  do #copies habanero and smooth DLLs to faces lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'temp/Habanero Community/Faces/trunk/lib'
	FileUtils.cp Dir.glob('temp/Habanero Community/SmoothHabanero/trunk/bin/Habanero.Smooth*.dll'), 'temp/Habanero Community/Faces/trunk/lib'
	end
  
    task :clean_faces do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Habanero Community/Faces/trunk/bin'
end

exec :checkout_faces do |cmd| #command to check out faces source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero Community/Faces/trunk/" "temp/Habanero Community/Faces/trunk/" --username chilli --password chilli --non-interactive)
end

msbuild :msdo_faces do |msb| #builds faces with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe" #faces trunk at the time of the creation of this script was using .Net 4 features
	msb.verbosity = "quiet"
  msb.solution = "temp/Habanero Community/Faces/trunk/source/Habanero.Faces - 2010.sln"
  end
  
  
#do_testability tasks
  task :copy_dlls_to_testability_lib  do #copies habanero and smooth DLLs to testability lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero Community/SmoothHabanero/trunk/bin/Habanero.Smooth*.dll'), 'lib'
	end
  
    task :clean_testability do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Habanero Community/Habanero.Testability/Trunk/bin'
end

msbuild :msdo_testability do |msb| #builds testability with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
  msb.solution = "source/Habanero.Testability - 2010.sln"
  end
  
 nunit :run_nunit do |nunit|
 nunit.path_to_command = $Nunit_path
 nunit.assemblies 'bin\Habanero.Testability.Tests.dll','bin\Habanero.Testability.Testers.Tests.dll'
 nunit.options '/xml=nunit-result.xml'
end
  
task :clean_up do
FileUtils.rm_rf 'temp'
end

exec :commit_lib do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(ci -m autocheckin --username chilli --password chilli) 
end
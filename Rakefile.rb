#Habanero.Testability v1.3-13_02_2012
require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "/rake-tasks")
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version ='9.9.9.999'
#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------
#------------------------project settings------------------------
$solution = 'source/Habanero.Testability - 2010.sln'
$solutionNuget = '"source/Habanero.Testability - 2010.sln"'
$major_version = ''
$minor_version = ''
$patch_version = ''
$nuget_apikey = ''
$nuget_sourceurl = ''
$nuget_publish_version = 'Trunk'

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------
desc "Runs the build all task"
task :default, [:major, :minor, :patch] => [:updatesubmodules,:setupvars, :build]

desc "Pulls habanero deps from local nuget, builds , tests and pushes testability"
task :build_test_push_internal, [:major, :minor, :patch, :apikey, :sourceurl] => [:updatesubmodules,:setupvars, :installNugetPackages, :build, :nugetpush]

desc "Builds Testability, including tests"
task :build, [:major, :minor, :patch]  => [:clean, :restorepackages, :setupvars, :set_assembly_version, :msbuild, :copy_to_nuget, :test]

#------------------------Setup Versions---------
desc "Setup Variables"
task :setupvars,:major ,:minor,:patch, :apikey, :sourceurl do |t, args|
	puts cyan("Setup Variables")
	args.with_defaults(:major => "0")
	args.with_defaults(:minor => "0")
	args.with_defaults(:patch => "0000")
	args.with_defaults(:apikey => "")
	args.with_defaults(:sourceurl => "")
	$major_version = "#{args[:major]}"
	$minor_version = "#{args[:minor]}"
	$patch_version = "#{args[:patch]}"
	$nuget_apikey = "#{args[:apikey]}"
	$nuget_sourceurl = "#{args[:sourceurl]}"
	$app_version = "#{$major_version}.#{$minor_version}.#{$patch_version}.0"
	puts cyan("Assembly Version #{$app_version}")
	puts cyan("Nuget key: #{$nuget_apikey} for: #{$nuget_sourceurl}")
end

desc "Restore Nuget Packages"
task :restorepackages do
	puts cyan('lib\nuget.exe restore '+"#{$solutionNuget}")
	system 'lib\nuget.exe restore '+"#{$solutionNuget}"
end

desc "Update Submodules"
task :updatesubmodules do
	puts cyan("Updating Git Submodules")
	system 'git submodule foreach git checkout master'
	system 'git submodule foreach git pull'
end

task :set_assembly_version do
	puts green("Setting Shared AssemblyVersion to: #{$app_version}")
	file_path = "source/Common/AssemblyInfoShared.cs"
	outdata = File.open(file_path).read.gsub(/"9.9.9.999"/, "\"#{$app_version}\"")
	File.open(file_path, 'w') do |out|
		out << outdata
	end	
end
#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
	FileUtils.rm_rf $nuget_baselocation	
	FileSystem.ensure_dir_exists $nuget_baselocation
end

desc "Builds the solution with msbuild"
msbuild :msbuild do |msb| 
	puts cyan("Building #{$solution} with msbuild")
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

desc "Runs the tests"
nunit :test do |nunit|
	puts cyan("Running tests")
	nunit.assemblies 'bin\Habanero.Testability.Tests.dll',
					 'bin\Habanero.Testability.Testers.Tests.dll'
end


def copy_nuget_files_to location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.dll", location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.Helpers.dll", location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.Testers.dll", location
end

task :copy_to_nuget do
	puts cyan("Copying files to the nuget folder")	
	copy_nuget_files_to $nuget_baselocation
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
	puts cyan("Install local nuget packages, Version : #{$nuget_publish_version}")
    ip.package_names = ["Habanero.Base.#{$nuget_publish_version}",  
						"Habanero.BO.#{$nuget_publish_version}",  
						"Habanero.Smooth.#{$nuget_publish_version}"]
	ip.SourceUrl = "#{$nuget_sourceurl}/nuget"
	ip.Version = $app_version
end

desc "Pushes Testability to Nuget"
task :nugetpush => [:publishTestabilityNugetPackage, 
					:publishTestabilityHelpersNugetPackage, 
					:publishTestabilityTestersNugetPackage ]
				
desc "Publish the Habanero.Testability nuget package"
pushnugetpackagesonline :publishTestabilityNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.dll"
  package.Nugetid = "Habanero.Testability.#{$nuget_publish_version}"
  package.Version = $app_version
  package.Description = "Testability"
  package.ApiKey = "#{$nuget_apikey}"
  package.SourceUrl = "#{$nuget_sourceurl}"
end

desc "Publish the Habanero.Testability.Helpers nuget package"
pushnugetpackagesonline :publishTestabilityHelpersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Helpers.dll"
  package.Nugetid = "Habanero.Testability.Helpers.#{$nuget_publish_version}"
  package.Version = $app_version
  package.Description = "Habanero.Testability.Helpers"
  package.ApiKey = "#{$nuget_apikey}"
  package.SourceUrl = "#{$nuget_sourceurl}"
end

desc "Publish the Habanero.Testability.Testers nuget package"
pushnugetpackagesonline :publishTestabilityTestersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Testers.dll"
  package.Nugetid = "Habanero.Testability.Testers.#{$nuget_publish_version}"
  package.Version = $app_version
  package.Description = "Habanero.Testability.Testers"
  package.ApiKey = "#{$nuget_apikey}"
  package.SourceUrl = "#{$nuget_sourceurl}"
end
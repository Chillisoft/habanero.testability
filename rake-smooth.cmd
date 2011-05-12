@cd temp/smooth 
@rmdir /s /q .svn 
rake --rakefile smooth-library-rakefile.rb --execute-continue "$smooth_version = 'branches/V2.6-CF_Stargate'; $buildscriptpath = 'F:/Systems/HabaneroCommunity/BuildScripts'" 

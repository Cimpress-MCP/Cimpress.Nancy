require 'bundler/setup'
require 'rake'
require 'rake/clean'
require 'fileutils'

STDOUT.sync
STDERR.sync

PACKAGE_NAME = 'NAN.Mancy2'
BUILD_CONFIGURATION = ENV['JOB_NAME'] ? 'Release' : ENV['BUILD_CONFIGURATION'] || 'Debug'

PWD = File.dirname(__FILE__)
SRC_DIR = File.join(PWD, 'src')
OUTPUT_DIR = File.join(PWD, 'output')
BUILD_DIR = File.join(OUTPUT_DIR, 'build')

directory BUILD_DIR => [OUTPUT_DIR]

# == Basic definitions used by several tasks

# By default, Rake's :clean task deletes anything named 'core'. How annoying!
CLEAN.include('src/**/bin', 'src/**/obj')

# == Workflow tasks

task :default => [:build]

desc "Builds the solution."
task :build => [:build_solution]

desc 'Builds the package.'
task :package => [:retrieve, :build]

desc 'Rebuilds the package.'
task :repackage => [:clean, :package]

desc "Run me for pull request jobs"
task :pull_requests_job => [:package]

desc "Run me for commit build jobs"
task :commit_job => [:package]


desc "Print help."
task :help do
    puts "Type 'rake -T' for task list or 'rake -D' for detailed help."
    Dir.chdir(File.dirname(__FILE__)) do
        sh "rake -T"
    end
end

# == Task definitions
task :retrieve do |t|
  raise "Could not retrieve NuGet dependencies" if !system("dotnet restore")
end

task :build_solution => [BUILD_DIR] do |t|
  raise "Failed to build solution" if !system("dotnet pack --configuration #{BUILD_CONFIGURATION} --version-suffix #{ENV["BUILD_NUMBER"] || "0"} --output #{OUTPUT_DIR} #{SRC_DIR}/project.json")
end

task :clobber do
  system('git clean -dfx')
end
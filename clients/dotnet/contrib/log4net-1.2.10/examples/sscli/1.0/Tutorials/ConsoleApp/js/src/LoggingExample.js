//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

// JScript.NET does not support application entry points (like vb.Net and C#), 
// instead it supports global code.
ConsoleApp.LoggingExample.Main();

import System;

import log4net;

// Configure log4net using the .config file
[assembly:log4net.Config.XmlConfigurator()]
// This will cause log4net to look for a configuration file
// called ConsoleApp.exe.config in the application base
// directory (i.e. the directory containing ConsoleApp.exe)

package ConsoleApp {
	// Example of how to simply configure and use log4net
	public class LoggingExample {
		// Create a logger for use in this class
		private static var log : log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		// NOTE that using System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
		// is equivalent to typeof(LoggingExample) but is more portable
		// i.e. you can copy the code directly into another class without
		// needing to edit the code.

		// Application entry point
		public static function Main() {
			// Log an info level message
			if (log.IsInfoEnabled) log.Info("Application [ConsoleApp] Start");

			// Log a debug message. Test if debug is enabled before
			// attempting to log the message. This is not required but
			// can make running without logging faster.
			if (log.IsDebugEnabled) log.Debug("This is a debug message");

			try {
				Bar();
			} catch (ex : Exception) {
				// Log an error with an exception
				log.Error("Exception thrown from method Bar", ex);
			}

			log.Error("Hey this is an error!");

			var disposableFrame : IDisposable;

			try {
				// Push a message on to the Nested Diagnostic Context stack
				disposableFrame = log4net.NDC.Push("NDC_Message");

				log.Warn("This should have an NDC message");

				// Set a Mapped Diagnostic Context value  
				log4net.MDC.Set("auth", "auth-none");
				log.Warn("This should have an MDC message for the key 'auth'");
			} finally {
				// The NDC message is popped off the stack by using the Dispose method
   				if (disposableFrame != null) disposableFrame.Dispose();
			}

			log.Warn("See the NDC has been popped of! The MDC 'auth' key is still with us.");

			// Log an info level message
			if (log.IsInfoEnabled) log.Info("Application [ConsoleApp] End");

			Console.Write("Press Enter to exit...");
			Console.ReadLine();
		}

		// Helper methods to demonstrate location information and nested exceptions

		private static function Bar() {
			Goo();
		}

		private static function Foo() {
			throw new Exception("This is an Exception");
		}

		private static function Goo() {
			try {
				Foo();
			} catch (ex : Exception) {
				throw new ArithmeticException("Failed in Goo. Calling Foo. Inner Exception provided", ex);
			}
		}
	}
}

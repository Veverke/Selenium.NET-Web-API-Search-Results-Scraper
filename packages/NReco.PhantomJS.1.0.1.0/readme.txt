NReco.PhantomJS v.1.0.0
-----------------------
Visit http://www.nrecosite.com/phantomjs_wrapper_net.aspx for the latest information (change log, examples, etc)
API documentation: http://www.nrecosite.com/doc/NReco.PhantomJS/

How to use
----------
using NReco.PhantomJS;

...

var phantomJS = new PhantomJS();
phantomJS.OutputReceived += (sender, e) => {
	Console.WriteLine("PhantomJS output: {0}", e.Data);
};
phantomJS.RunScript("for (var i=0; i<10; i++) console.log('hello from js '+i); phantom.exit();", null);
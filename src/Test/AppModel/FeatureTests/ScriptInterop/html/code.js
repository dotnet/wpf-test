            function ArgTest(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17)
            {
                return typeof(arg1) + typeof(arg2) + typeof(arg3) + typeof(arg4) + typeof(arg5) + typeof(arg6) + typeof(arg7) + typeof(arg8) + typeof(arg9) +
                    typeof(arg10) + typeof(arg11) + typeof(arg12) + typeof(arg13) + typeof(arg14) + typeof(arg15) + typeof(arg16) + typeof(arg17);
            } 

            function ArrayTest(inputArray)
            {
                return inputArray;
            }

            function CallbackTest(callbackObj) 
            {
                callbackObj.Callback("Called from JavaScript!");
            }


            function DateTimeTest(dateTime)
            {
               date = new Date(dateTime);
               return "" + date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();
            }
  

            function JsonTest()
            {
                return { name: "Nerf Herder", number: 1 };
            }

            function ManagedException(callbackObj)
            {
                try
                {
                    callbackObj.CauseException();
                }
                catch(e)
                {
                    if (e.description == null) return e.toString();
                    return e.description;
                }
                return null;
            }

            function ReturnArgsTest()
            {
                //note: considered adding unknown, but it doesn't work and likely can't be created in Javascript.
                return new Array(true, 3.14, 1e+308, 1e+309,1e-1000,-1e+309, "test", null, undefined, Infinity, -Infinity, NaN);
            }

            function ReturnJsFunction()
            {
                return TestAdd;
            }

            function RoundTripTest(arg)
            {
                return arg;
            }

            function ScriptEventTest(arg)
            {
                btn1.onclick = arg;
                btn1.fireEvent("onclick");
                return arg;
            }

            function ScriptExceptionTest()
            {
                throw new Error("danger will");
            }

            function ScriptRoundTripTest(managedObject)
            {
                managedObject.ArgTypes(true, 42, 1e+3100, "test", null);
            }

            function VerifyRoundTrip(arg1, arg2, arg3, arg4, arg5)
            {
                return(typeof(arg1) + typeof(arg2) + typeof(arg3) + typeof(arg4) + typeof(arg5));
            }

            function TestAdd(arg1, arg2)
            {
                return arg1 + arg2;
            }

            savedObject = null;

            function SetSavedObject(arg)
            {
                savedObject = arg;
            }
 
            function GetSavedObject()
            {
                return savedObject;
            }

            scriptVariable = null;

            function UnpackArg(jsonObject)
            {
                return jsonObject.name;
            }

            function WrongNumberOfArgsTest(managedObject)
            {
                try
                {
                    managedObject.Callback();
                }
                catch(e)
                {
                    if (e.number == null) result = e.toString();
                    else result = e.number;
                }
                try
                {
                    managedObject.Callback("test","passed"); 
                }
                catch(e)
                {
                    if (e.number == null) result += " / " + e.toString();
                    else result += " / " + e.number;
                }

                return result;
            }

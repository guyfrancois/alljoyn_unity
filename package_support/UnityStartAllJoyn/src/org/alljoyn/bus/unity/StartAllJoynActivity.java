/*
 * Copyright 2012, Qualcomm Innovation Center, Inc.
 * 
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 * 
 *        http://www.apache.org/licenses/LICENSE-2.0
 * 
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
package org.alljoyn.bus.unity;

import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;
public class StartAllJoynActivity extends UnityPlayerActivity {
/*
* The import and class definition below are to be used if a Vuforia Unity project
* is using the AllJoyn Unity extension.  Comment out the 2 lines above and comment in
* the lines below.  unitystartalljoyn.jar will automatically be generated in the bin
* folder
*/
//import com.qualcomm.QCARUnityPlayer.QCARPlayerProxyActivity;
//public class StartAllJoynActivity extends QCARPlayerProxyActivity {

	static 
    {
        System.loadLibrary("alljoyn_java");
    }
	
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
    	org.alljoyn.bus.alljoyn.DaemonInit.PrepareDaemon(getApplicationContext());
  	}

} 


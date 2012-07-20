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


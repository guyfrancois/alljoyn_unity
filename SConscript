# Copyright 2010 - 2011, 2013, Qualcomm Innovation Center, Inc.
# 
#    Licensed under the Apache License, Version 2.0 (the "License");
#    you may not use this file except in compliance with the License.
#    You may obtain a copy of the License at
# 
#        http://www.apache.org/licenses/LICENSE-2.0
# 
#    Unless required by applicable law or agreed to in writing, software
#    distributed under the License is distributed on an "AS IS" BASIS,
#    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#    See the License for the specific language governing permissions and
#    limitations under the License.
# 

import os
import sys

Import('env')

# Dependent Projects
if not env.has_key('_ALLJOYNC_'):
    # alljoyn_c is required for the unity_bindings
    env.SConscript('../alljoyn_c/SConscript')

unityenv = env.Clone()

vars = Variables();
#TODO place any environment vars here
vars.Update(unityenv)
Help(vars.GenerateHelpText(unityenv))

sys.path.append(str(unityenv.Dir('../build_core/tools/scons').srcnode()))

# Make alljoyn_unity dist a sub-directory of the alljoyn dist.  This avoids any conflicts with alljoyn dist targets.
unityenv['UNITY_DISTDIR'] = unityenv['DISTDIR'] + '/unity'
unityenv['UNITY_TESTDIR'] = unityenv['TESTDIR'] + '/unity'

# Add support for multiple build targets in the same workset
unityenv.VariantDir('$OBJDIR', 'src', duplicate = 0)
unityenv.VariantDir('$OBJDIR/samples', 'samples', duplicate = 0)

#alljoyn_unity.dll
unityenv['ALLJOYN_UNITY_LIB'] = unityenv.SConscript('src/SConscript', exports = {'env':unityenv})

# Sample programs
progs = unityenv.SConscript('samples/SConscript', exports = {'env':unityenv})

#install Package support files
package_support_dir = unityenv.Dir('package_support/UnityPackage')
unityenv.Install('$UNITY_DISTDIR/package_support', package_support_dir)

#install libraries into the package support directory
if unityenv['OS_GROUP'] == 'windows':
    #place alljoyn_unity.dll into the samples
    unityenv.Install('package_support/UnityPackage/Assets/Plugins', unityenv['ALLJOYN_UNITY_LIB'])

    #place alljoyn_c.dll into the samples
    liballjoyn_c = '$DISTDIR/c/lib/alljoyn_c.dll'
    unityenv.Install('package_support/UnityPackage/Assets/Plugins', liballjoyn_c)

if unityenv['OS'] == 'android':
    #place alljoyn_unity.dll into the samples
    unityenv.Install('package_support/UnityPackage/Assets/Plugins', unityenv['ALLJOYN_UNITY_LIB'])

    #place liballjoyn_c.so into the samples
    liballjoyn_c = '$DISTDIR/c/lib/liballjoyn_c.so'
    unityenv.Install('package_support/UnityPackage/Assets/Plugins/Android', liballjoyn_c)

#Build unit tests
unityenv.SConscript('unit_test/SConscript', exports = {'env':unityenv})
# Build docs
unityenv.SConscript('docs/SConscript', exports = {'env':unityenv})

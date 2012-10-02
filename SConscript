# Copyright 2010 - 2011, Qualcomm Innovation Center, Inc.
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

vars = Variables();
#TODO place any environment vars here
vars.Update(env)

Help(vars.GenerateHelpText(env))

sys.path.append('../build_core/tools/scons')

# Dependent Projects
if not env.has_key('_ALLJOYNCORE_'):
    #alljoyn_c is required for the unity_bindings
    env.SConscript('../alljoyn_c/SConscript')

# Make alljoyn_unity dist a sub-directory of the alljoyn dist.  This avoids any conflicts with alljoyn dist targets.
env['UNITY_DISTDIR'] = env['DISTDIR'] + '/unity'
env['UNITY_TESTDIR'] = env['TESTDIR'] + '/unity'

# Add support for multiple build targets in the same workset
env.VariantDir('$OBJDIR', 'src', duplicate = 0)
env.VariantDir('$OBJDIR/samples', 'samples', duplicate = 0)

#alljoyn_unity.dll
env['ALLJOYN_UNITY_LIB'] = env.SConscript('src/SConscript')

# Sample programs
progs = env.SConscript('samples/SConscript')
#env.Install('$UNITY_DISTDIR/bin/samples', progs)

#install Package support files
package_support_dir = env.Dir('package_support/UnityPackage')
env.Install('$UNITY_DISTDIR/package_support', package_support_dir)

#install libraries into the package support directory
if env['OS_GROUP'] == 'windows':
    #place alljoyn_unity.dll into the samples
    env.Install('package_support/UnityPackage/Assets/Plugins', env['ALLJOYN_UNITY_LIB'])

    #place alljoyn_c.dll into the samples
    liballjoyn_c = '$DISTDIR/lib/alljoyn_c.dll'
    env.Install('package_support/UnityPackage/Assets/Plugins', liballjoyn_c)

if env['OS'] == 'android':
    #place alljoyn_unity.dll into the samples
    env.Install('package_support/UnityPackage/Assets/Plugins', env['ALLJOYN_UNITY_LIB'])

    #place liballjoyn_c.so into the samples
    liballjoyn_c = '$DISTDIR/lib/liballjoyn_c.so'
    env.Install('package_support/UnityPackage/Assets/Plugins/Android', liballjoyn_c)

# Build docs
env.SConscript('docs/SConscript')
    

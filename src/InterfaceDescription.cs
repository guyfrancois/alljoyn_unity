/**
 * @file
 * This file defines types for statically describing a message bus interface
 */

/******************************************************************************
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
 ******************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		/**
		 * @class InterfaceDescription
		 * Class for describing message bus interfaces. %InterfaceDescription objects describe the methods,
		 * signals and properties of a BusObject or ProxyBusObject.
		 *
		 * Calling ProxyBusObject::AddInterface(const char*) adds the AllJoyn interface described by an
		 * %InterfaceDescription to a ProxyBusObject instance. After an  %InterfaceDescription has been
		 * added, the methods described in the interface can be called. Similarly calling
		 * BusObject::AddInterface adds the interface and its methods, properties, and signal to a
		 * BusObject. After an interface has been added method handlers for the methods described in the
		 * interface can be added by calling BusObject::AddMethodHandler or BusObject::AddMethodHandlers.
		 *
		 * An %InterfaceDescription can be constructed piecemeal by calling InterfaceDescription::AddMethod,
		 * InterfaceDescription::AddMember(), and InterfaceDescription::AddProperty(). Alternatively,
		 * calling ProxyBusObject::ParseXml will create the %InterfaceDescription instances for that proxy
		 * object directly from an XML string. Calling ProxyBusObject::IntrospectRemoteObject or
		 * ProxyBusObject::IntrospectRemoteObjectAsync also creates the %InterfaceDescription
		 * instances from XML but in this case the XML is obtained by making a remote Introspect method
		 * call on a bus object.
		 */
		public class InterfaceDescription
		{
			[Flags]
			/** @name Annotation flags */
			public enum AnnotationFlags : byte
			{
				Default = 0, /**< Default annotate flag */
				NoReply = 1, /**< No reply annotate flag */
				Deprecated = 2 /**< Deprecated annotate flag */
			}

			[Flags]
			/** @name Access type */
			public enum AccessFlags : byte
			{
				Read = 1, /**< Read Access type */
				Write = 2, /**< Write Access type */
				ReadWrite = 3 /**< Read-Write Access type */
			}

			internal InterfaceDescription(IntPtr interfaceDescription)
			{
			
				_interfaceDescription = interfaceDescription;
			}

			#region Equality
			/**
			 * Equality operation.
			 *
			 * @param one	InterfaceDescription to be compared to
			 * @param other	InterfaceDescription to be compared against
			 *
			 * @return true if InterfaceDescriptions are equal
			 */
			public static bool operator ==(InterfaceDescription one, InterfaceDescription other)
			{
			
				if((object)one == null && (object)other == null) return true;
				else if((object)one == null || (object)other == null) return false;
				return (alljoyn_interfacedescription_eql(one.UnmanagedPtr, other.UnmanagedPtr) == 1 ? true : false);
			}

			/**
			 * Not Equality operation.
			 *
			 * @param one	InterfaceDescription to be compared to
			 * @param other	InterfaceDescription to be compared against
			 *
			 * @return true if InterfaceDescriptions are not equal
			 */public static bool operator !=(InterfaceDescription one, InterfaceDescription other)
			{
			
				return !(one == other);
			}

			/**
			 * Equality operation.
			 *
			 * @param o	InterfaceDescription to be compared against
			 *
			 * @return true if InterfaceDescriptions are equal
			 */
			public override bool Equals(object o) 
			{
				try
				{
					return (this == (InterfaceDescription)o);
				}
				catch
				{
					return false;
				}
			}

			/**
			 * Returns the raw pointer of the interfaceDescription.
			 *
			 * @return the raw pointer of the interfaceDescription
			 */
			public override int GetHashCode()
			{
			
				return (int)_interfaceDescription;
			}
			#endregion

			#region Properties
			/**
			 * Check for existence of any properties
			 *
			 * @return  true if interface has any properties.
			 */
			public bool HasProperties
			{
				get
				{
					return (alljoyn_interfacedescription_hasproperties(_interfaceDescription) == 1 ? true : false);
				}
			}

			/**
			 * Returns the name of the interface
			 *
			 * @return the interface name.
			 */
			public string Name
			{
				get
				{
					return Marshal.PtrToStringAnsi(alljoyn_interfacedescription_getname(_interfaceDescription));
				}
			}

			/**
			 * Indicates if this interface is secure. Secure interfaces require end-to-end authentication.
			 * The arguments for methods calls made to secure interfaces and signals emitted by secure
			 * interfaces are encrypted.
			 * @return true if the interface is secure.
			 */
			public bool IsSecure
			{
				get
				{
					return (alljoyn_interfacedescription_issecure(_interfaceDescription) == 1 ? true : false);
				}
			}
			#endregion
			/**
			     * Add a member to the interface.
			     *
			     * @param type        Message type.
			     * @param name        Name of member.
			     * @param inputSignature    Signature of input parameters or NULL for none.
			     * @param outputSignature      Signature of output parameters or NULL for none.
			     * @param argNames    Comma separated list of input and then output arg names used in annotation XML.
			     *
			     * @return
			     *      - #ER_OK if successful
			     *      - #ER_BUS_MEMBER_ALREADY_EXISTS if member already exists
			     */
			public QStatus AddMember(Message.Type type, string name, string inputSignature,
				string outputSignature, string argNames)
			{
			
				return alljoyn_interfacedescription_addmember(_interfaceDescription,
					(int)type, name, inputSignature, outputSignature, argNames, (byte)AnnotationFlags.Default);
			}

			/**
			     * Add a member to the interface.
			     *
			     * @param type        Message type.
			     * @param name        Name of member.
			     * @param inputSignature    Signature of input parameters or NULL for none.
			     * @param outputSignature      Signature of output parameters or NULL for none.
			     * @param argNames    Comma separated list of input and then output arg names used in annotation XML.
			     * @param annotation  Annotation flags.
			     *
			     * @return
			     *      - #ER_OK if successful
			     *      - #ER_BUS_MEMBER_ALREADY_EXISTS if member already exists
			     */
			public QStatus AddMember(Message.Type type, string name, string inputSignature,
				string outputSignature, string argNames, AnnotationFlags annotation)
			{
			
				return alljoyn_interfacedescription_addmember(_interfaceDescription,
					(int)type, name, inputSignature, outputSignature, argNames, (byte)annotation);
			}

			/**
			     * Add a signal member to the interface.
			     *
			     * @param name        Name of method call member.
			     * @param inputSignature         Signature of parameters or NULL for none.
			     * @param argNames    Comma separated list of arg names used in annotation XML.
			     * @param annotation  Annotation flags.
			     *
			     * @return
			     *      - #ER_OK if successful
			     *      - #ER_BUS_MEMBER_ALREADY_EXISTS if member already exists
			     */
		    public QStatus AddSignal(string name, string inputSignature, string argNames, AnnotationFlags annotation)
		    {
			return alljoyn_interfacedescription_addmember(_interfaceDescription,
			    (int)Message.Type.Signal, name, inputSignature, null, argNames, (byte) annotation);
		    }

			/**
			     * Activate this interface. An interface must be activated before it can be used. Activating an
			     * interface locks the interface so that is can no longer be modified.
			     */
			public void Activate()
			{
			
				alljoyn_interfacedescription_activate(_interfaceDescription);
			}

			/**
			     * Lookup a member description by name
			     *
			     * @param name  Name of the member to lookup
			     * @return
			     *      - Pointer to member.
			     *      - NULL if does not exist.
			     */
			public Member GetMember(string name)
			{
			
				_Member retMember = new _Member();
				if(alljoyn_interfacedescription_getmember(_interfaceDescription, name, ref retMember) == 1)
				{
					return new Member(retMember);
				}
				else
				{
					return null;
				}
			}

			/**
			     * Get all the members.
			     *
			     * @return  The members array to receive the members.
			     */
			public Member[] GetMembers()
			{
			
				UIntPtr numMembers = alljoyn_interfacedescription_getmembers(_interfaceDescription,
					IntPtr.Zero, (UIntPtr)0);
				_Member[] members = new _Member[(int)numMembers];
				GCHandle gch = GCHandle.Alloc(members, GCHandleType.Pinned);
				UIntPtr numFilledMembers = alljoyn_interfacedescription_getmembers(_interfaceDescription,
					gch.AddrOfPinnedObject(), numMembers);
				if(numMembers != numFilledMembers)
				{
					// Warn?
				}
				Member[] ret = new Member[(int)numFilledMembers];
				for(int i = 0; i < ret.Length; i++)
				{
					ret[i] = new Member(members[i]);
				}

				return ret;
			}

			/**
			     * Check for existence of a member. Optionally check the signature also.
			     * @remark
			     * if the a signature is not provided this method will only check to see if
			     * a member with the given @c name exists.  If a signature is provided a
			     * member with the given @c name and @c signature must exist for this to return true.
			     *
			     * @param name       Name of the member to lookup
			     * @param inSig      Input parameter signature of the member to lookup
			     * @param outSig     Output parameter signature of the member to lookup (leave NULL for signals)
			     * @return true if the member name exists.
			     */
			public bool HasMember(string name, string inSig, string outSig)
			{
			
				return (alljoyn_interfacedescription_hasmember(_interfaceDescription,
					name, inSig, outSig) == 1 ? true : false );
			}

			/**
			     * Lookup a property description by name
			     *
			     * @param name  Name of the property to lookup
			     * @return a structure representing the properties of the interface
			     */
			public Property GetProperty(string name)
			{
			
				_Property retProp = new _Property();
				if(alljoyn_interfacedescription_getproperty(_interfaceDescription, name, ref retProp) == 1)
				{
					return new Property(retProp);
				}
				else
				{
					return null;
				}
			}

			/**
			     * Get all the properties.
			     *
			     * @return  The property array that represents the available properties.
			     */
			public Property[] GetProperties()
			{
			
				UIntPtr numProperties = alljoyn_interfacedescription_getproperties(_interfaceDescription,
					IntPtr.Zero, (UIntPtr)0);
				_Property[] props = new _Property[(int)numProperties];
				GCHandle gch = GCHandle.Alloc(props, GCHandleType.Pinned);
				UIntPtr numFilledProperties = alljoyn_interfacedescription_getproperties(_interfaceDescription,
					gch.AddrOfPinnedObject(), numProperties);
				if(numProperties != numFilledProperties)
				{
					// Warn?
				}
				Property[] ret = new Property[(int)numFilledProperties];
				for(int i = 0; i < ret.Length; i++)
				{
					ret[i] = new Property(props[i]);
				}

				return ret;
			}

			/**
			     * Add a property to the interface.
			     *
			     * @param name       Name of property.
			     * @param signature  Property type.
			     * @param access     #PROP_ACCESS_READ, #PROP_ACCESS_WRITE or #PROP_ACCESS_RW
			     * @return
			     *      - #ER_OK if successful.
			     *      - #ER_BUS_PROPERTY_ALREADY_EXISTS if the property can not be added
			     *                                        because it already exists.
			     */
			public QStatus AddProperty(string name, string signature, AccessFlags access)
			{
			
				return alljoyn_interfacedescription_addproperty(_interfaceDescription,
					name, signature, (byte)access);
			}

			/**
			     * Check for existence of a property.
			     *
			     * @param name       Name of the property to lookup
			     * @return true if the property exists.
			     */
			public bool HasProperty(string name)
			{
			
				return (alljoyn_interfacedescription_hasproperty(_interfaceDescription, name) == 1 ? true : false);
			}

			public class Member
			{
				public InterfaceDescription Iface
				{
					get
					{
						return new InterfaceDescription(_member.iface);
					}
				}

				public Message.Type MemberType
				{
					get
					{
						return (Message.Type)_member.memberType;
					}
				}

				public string Name
				{
					get
					{
						return Marshal.PtrToStringAnsi(_member.name);
					}
				}

				public string Signature
				{
					get
					{
						return Marshal.PtrToStringAnsi(_member.signature);
					}
				}

				public string ReturnSignature
				{
					get
					{
						return Marshal.PtrToStringAnsi(_member.returnSignature);
					}
				}

				public string ArgNames
				{
					get
					{
						return Marshal.PtrToStringAnsi(_member.argNames);
					}
				}

				public AnnotationFlags Annotation
				{
					get
					{
						return (AnnotationFlags)_member.annotation;
					}
				}

				internal Member(_Member member)
				{
					_member = member;
				}

				internal Member(IntPtr memberPtr)
				{
					_member = (_Member)Marshal.PtrToStructure(memberPtr, typeof(_Member));
				}

				#region Data
				internal _Member _member;
				#endregion
			}

			public class Property
			{
				public string Name
				{
					get
					{
						return Marshal.PtrToStringAnsi(_property.name);
					}
				}

				public string Signature
				{
					get
					{
						return Marshal.PtrToStringAnsi(_property.signature);
					}
				}

				public AccessFlags Access
				{
					get
					{
						return (AccessFlags)_property.access;
					}
				}

				internal Property(_Property property)
				{
					_property = property;
				}

				internal Property(IntPtr propertyPointer)
				{
					_property = (_Property)Marshal.PtrToStructure(propertyPointer, typeof(_Property));
				}

				#region Data
				internal _Property _property;
				#endregion
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_addmember(
				IntPtr iface,
				int type,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string inputSig,
				[MarshalAs(UnmanagedType.LPStr)] string outSig,
				[MarshalAs(UnmanagedType.LPStr)] string argNames,
				byte annotation);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_interfacedescription_activate(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_getmember(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref _Member member);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_interfacedescription_getmembers(IntPtr iface,
				IntPtr members, UIntPtr numMembers);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasmember(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string inSig,
				[MarshalAs(UnmanagedType.LPStr)] string outSig);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_getproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref _Property property);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_interfacedescription_getproperties(IntPtr iface,
				IntPtr props, UIntPtr numProps);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_addproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string signature,
				byte access);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_eql(IntPtr one, IntPtr other);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasproperties(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_interfacedescription_getname(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_issecure(IntPtr iface);
			#endregion

			#region Internal Structures
			[StructLayout(LayoutKind.Sequential)]
			internal struct _Member
			{
				public IntPtr iface;
				public int memberType;
				public IntPtr name;
				public IntPtr signature;
				public IntPtr returnSignature;
				public IntPtr argNames;
				public byte annotation;
#pragma warning disable 169 // Field is never used
				public IntPtr internal_member;
#pragma warning restore 169
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct _Property
			{
				public IntPtr name;
				public IntPtr signature;
				public byte access;
#pragma warning disable 169 // Field is never used
				private IntPtr internal_property;
#pragma warning restore 169
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _interfaceDescription;
				}
			}
			#endregion

			#region Data
			IntPtr _interfaceDescription;
			#endregion
		}
	}
}


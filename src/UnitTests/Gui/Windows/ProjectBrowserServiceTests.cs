﻿#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Controls;
using System.ComponentModel;
using System.ComponentModel.Design;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class ProjectBrowserServiceTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private FakeTreeView fakeTree;
        private ITreeView mockTree;
        private ITreeNodeCollection mockNodes;
        private IDecompilerService decompilerSvc;
        private IDecompiler decompiler;
        private Program program;
        private Program[] programs;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            mockTree = mr.StrictMock<ITreeView>();
            mockNodes = mr.StrictMock<ITreeNodeCollection>();
            decompilerSvc = mr.StrictMock<IDecompilerService>();
            decompiler = mr.StrictMock<IDecompiler>();
            mockTree.Stub(t => t.Nodes).Return(mockNodes);
            fakeTree = new FakeTreeView();
        }

        private void Expect(string sExp)
        {
            var x = new XElement("foo");
            Func<ITreeNode, XNode> render = null;
            render = new Func<ITreeNode, XNode>(n =>
            {
                var e = new XElement(
                    "node",
                    new XAttribute[] {
                        n.Text != null ? new XAttribute("text", n.Text): null,
                        n.ToolTipText != null ? new XAttribute("tip", n.ToolTipText) : null,
                        n.Tag != null ? new XAttribute("tag", n.Tag.GetType().Name) : null
                    }.Where(a => a != null),
                    n.Nodes.Select(c => render(c)));
                    
                return e;
            });
            var sb = new StringWriter();
            var xdoc = new XDocument(
                new XElement("root",
                    fakeTree.Nodes.Select(n => render(n))));
            xdoc.RemoveAnnotations<XProcessingInstruction>();
            xdoc.WriteTo(new XmlTextWriter(sb));
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }


        private class FakeTreeView : ITreeView
        {
            public FakeTreeView()
            {
                this.Nodes = new FakeTreeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }
            public event EventHandler AfterSelect;

            public ITreeNode SelectedNode { get { return selectedItem; } set { selectedItem = value; AfterSelect.Fire(this); } }
            private ITreeNode selectedItem;

            public bool ShowRootLines { get; set; }
            public bool ShowNodeToolTips { get; set; }

            public ITreeNode CreateNode()
            {
                return new FakeTreeNode();
            }

            public ITreeNode CreateNode(string text)
            {
                return new FakeTreeNode { Text = text };
            }
        }

        private class FakeTreeNodeCollection : List<ITreeNode>,  ITreeNodeCollection
        {
            public ITreeNode Add(string text)
            {
                var node = new FakeTreeNode { Text = text };
                base.Add(node);
                return node;
            }
        }

        private class FakeTreeNode : ITreeNode
        {
            public FakeTreeNode()
            {
                Nodes = new FakeTreeNodeCollection();
            }

            public object Tag { get; set; }
            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Expand() { }
        }

        [Test]
        public void PBS_NoProject()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            pbs.Load(null, null);

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?><root><node text=\"(No project loaded)\" /></root>");
            Assert.IsFalse(fakeTree.ShowRootLines);
            Assert.IsFalse(fakeTree.ShowNodeToolTips);
        }

        private void Given_ProgramWithOneSegment()
        {
            var image = new LoadedImage(new Address(0x12340000), new byte[0x1000]);
            var imageMap = new ImageMap(image);
            imageMap.AddSegment(new Address(0x12340000), ".text", AccessMode.Execute);
            var arch = mr.StrictMock<IProcessorArchitecture>();
            var platform = new DefaultPlatform(sc, arch);
            program = new Program(image, imageMap, arch, platform);
            programs = new[] { program }; 
        }

        [Test]
        public void PBS_SingleBinary()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            Given_ProgramWithOneSegment();
            var project =  new Project
            {
                InputFiles = {
                    new InputFile {
                            Filename = "/home/fnord/project/executable",
                            BaseAddress = new Address(0x12340000),
                    }
                },
            };

            pbs.Load(project, programs);

            Assert.IsTrue(fakeTree.ShowNodeToolTips);
            var cr = Environment.NewLine == "\r\n"
                ? "&#xD;&#xA;"
                : "&#xA;";
            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"executable\" " +
                    "tip=\"/home/fnord/project/executable" + cr + "12340000\" " +
                    "tag=\"InputFile\">" +
                    "<node " + 
                        "text=\"Image base\" " +
                        "tip=\"Image base" + cr + "12340000" + cr + "" + cr + "\" " +
                        "tag=\"ImageMapSegment\" />" +
                "</node>" +
                "</root>");
        }

        [Test]
        public void PBS_AddBinary()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            Given_ProgramWithOneSegment();
            var project = new Project
            {
                InputFiles = 
                {
                    new InputFile { Filename="foo.exe", BaseAddress=new Address(0x400000) }
                }
            };
            pbs.Load(project, programs);
            mr.ReplayAll();

            project.InputFiles.Add(new InputFile
            {
                Filename = "bar.exe",
                BaseAddress = new Address(0x1231300)
            });

            Assert.AreEqual(2, fakeTree.Nodes.Count);
            mr.VerifyAll();
        }

        [Test]
        public void PBS_AfterSelect_Calls_DoDefaultAction()
        {
            var des = mr.StrictMock<TreeNodeDesigner>();
            var node = mr.Stub<ITreeNode>();
            des.Expect(d => d.DoDefaultAction());
            des.Stub(d => d.Initialize(null)).IgnoreArguments();
            mockTree = new FakeTree();
            mr.ReplayAll();
            
            var pbs = new ProjectBrowserService(sc, mockTree);
            pbs.AddComponents(new object[] { des });
            var desdes = pbs.GetDesigner(des);
            Assert.IsNotNull(desdes);

            mockTree.SelectedNode = des.TreeNode;

            mr.VerifyAll();
        }

        public class FakeTree : ITreeView
        {
            public event EventHandler AfterSelect;

            public FakeTree()
            {
                this.Nodes = new FakeNodeCollection();
            }

            public ITreeNode SelectedNode
            {
                get { return sel; }
                set { sel = value; AfterSelect.Fire(this); }
            }
            private ITreeNode sel;

            public bool ShowNodeToolTips { get; set; }
            public bool ShowRootLines { get; set; }

            public ITreeNodeCollection Nodes { get; private set; }

            public ITreeNode CreateNode()
            {
                return new FakeNode();
            }

            public ITreeNode CreateNode(string text)
            {
                throw new NotImplementedException();
            }
        }

        public class FakeNodeCollection : ITreeNodeCollection
        {
            private List<ITreeNode> nodes = new List<ITreeNode>();

            public ITreeNode Add(string text)
            {
                var node = new FakeNode { Text = text };
                nodes.Add(node);
                return node;
            }

            public void AddRange(IEnumerable<ITreeNode> nodes)
            {
                this.nodes.AddRange(nodes);
            }

            public int IndexOf(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public ITreeNode this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ITreeNode[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<ITreeNode> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class FakeNode : ITreeNode
        {
            public FakeNode()
            {
                Nodes = new FakeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public object Tag {get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Expand()
            {
            }
        }

        [Designer(typeof(TestDesigner))]
        public class TestComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class ParentComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class GrandParentComponent
        {
        }

        public class TestDesigner : TreeNodeDesigner
        {
        }

        [Test]
        public void PBS_FindGrandParent()
        {
            mockTree = new FakeTree();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var gp = new GrandParentComponent();
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { gp });
            pbs.AddComponents(gp, new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.AreSame(gp, o);
        }

        [Test]
        public void PBS_NoGrandParent()
        {
            mockTree = new FakeTree();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.IsNull(o);
        }
    }
}

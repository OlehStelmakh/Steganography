   M S B A M L   `   `   `  ���� H  DSteganography, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SteganographySteganography      Steganography.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:Steganography       � WindowStartupLocation$  CenterScreen=� � Title$ Steganography encryption��$	��600��$	��800�� � Icon$ Images/logo.png��$	��800��$	��450�� � DataContext $   Steganography.MainCalculations  .���  � ColumnDefinitions �� $��* ��� $	��200 � � RowDefinitions �� $
��3.7* �.+�� $��0��$��1�����	#FF404040 �� $��* ��� $
��50px �.+��� $��0��$��Stretch=�$��	0,0,0,0.5q�.+��� $��
10,10,10,5q�$	��Top=�$��40����Z� ��Z� ��Z� ���� *OpenFileDialogCommand+.����  �� Orientation$ 
Horizontal=�$��Right=�$	��170��.+��� $��24��$��24��$��Images/photo.png��$��10 0 5 0q�$��Center=��� $��Open an image��$��18(�
�������$��0q��� -   $��OpenTextForEncryption��$��	10,0,10,5q�$	��Top=�$��40����Z� ��Z� ��Z� ���� *OpenFileDialogTextCommand+.���� $ 
Horizontal=�$	��170��.+��� $��24��$��24��$��Images/text.png��$��10 0 5 0q�$��Center=��� $��	Open text��$��18(�
�������$��0q��� -   $��OpenFileKey��$��	10,0,10,5q�$	��Top=�$��40����Z� ��Z� ��Z� ���� *OpenFileDialogKeyCommand+.���� $ 
Horizontal=�$	��170��.+��� $��24��$��24��$��Images/key.png��$��10 0 5 0q�$��Center=��� $��Open key file��$��18(�
�������$��0q��� -   $��EncryptButton��$��	10,0,10,5q�$	��Top=�$��40����Z� ��Z� ��Z� ���� *EncryptCommand+.���� $ 
Horizontal=�$	��170��.+��� $��24��$��24��$��Images/unlock.png��$��10 0 5 0q�$��Center=��� $��Encrypt��$��18(�
�������$��0q��� -   $��DecryptButton��$��	10,0,10,5q�$	��Top=�$��40����Z� ��Z� ��Z� ���� *DecryptCommand+.���� $ 
Horizontal=�$	��170��.+��� $��24��$��24��$��Images/lock.png��$��10 0 5 0q�$��Center=��� $��Decrypt��$��18(�
�������$��0q��� $��	10 0 10 0q�
���2͚�$��2q�._��� .+��� $��10q�$,��&All 32-127 ASCII symbols are available��$��16(�
�������$
��Wrap=� �� TextAlignment$ Center=��� $��
10 0 10 10q�$��16(�
�������$
��Wrap=�$ Center=����� *	TextLimit+�� $��10q�$��18(�$
��Wrap=�$ Center=����� *
StatusText+���� *StatusColor+�� $��1��$��Stretch=�$��	0,0,0,0.5q�.+��� -   $��	10,0,10,5q�$��40��$��Bottom=���Z� ��Z� ��Z� .���� $ 
Horizontal=�$	��170��.+��� $��24��$��24��$��Images/close.png��$��10 0 5 0q�$��Center=��� $
��Exit��$��18(�
�������$��0q�� $��0��$��0�� �� $��3* ��� $
��Auto ��� $��* �.+��� 
����������	#FF404040$��2q�$��0��._��� -    ��Name$ 	ImageName������ *ImageBinding+	 �� Mode$	 OneWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=��� $��1�� �� ShowsPreview .  $��5��$��Stretch=�$��Center=����	#FF404040�� 
����������	#FF404040$��2q�$��2��._��� $��10 10 10 10q�" �� VerticalScrollBarVisibility$
 Auto=� �� TextWrapping$
 Wrap=� �� 	MaxLength �� *LengthLimitation+���� *	Message+$	 TwoWay=�
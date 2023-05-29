<?xml version="1.0" encoding="utf-16"?>
<IteModel iteVersion="MDEModel_2.1_" sourceTool="MDE" sourceModelPath="" modelName="TemplateModel" label="TemplateModel" sourceBinaryPath="" replacedInvalidIdentifiers="true" automated="false" referenceID="642091e3-c596-41d9-8816-50320d0ffff1" comment="" previewMode="false" stateBasedTestGeneration="1" xmlns="http://mbt.ntdev.corp.microsoft.com/mde/MDEModel_2.1_.xsd">
    <StateVariables referenceID="32f6bd7e-8b15-4cf3-b02a-f19e56bf2e81" />
    <ModelParameters referenceID="2bd75ff9-c3e4-4f72-8dc5-e8206ccdc764">
        <Parameters referenceID="eb96ce90-bbd0-4876-bc9b-e834c73c7c47">
            <Parameter key_Name="Kind" label="Kind" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="Kind of test to run." dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="f91b3683-ada7-481e-ba6e-c762448f6f65">
                <ParameterValue key_Name="CodeOnly" label="CodeOnly" weight="1" includeInGenerationType="0" resultWeight="1" comment="The tree will be constructed entirely in code.  The xaml parser will not be used." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d361a99f-ad54-42ff-807c-63daac340d64" />
                <ParameterValue key_Name="XamlCompile" label="XamlCompile" weight="1" includeInGenerationType="1" resultWeight="1" comment="The tree will be constructed in xaml, which will be compiled and run as a new process." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="a59a6f7e-699e-43cc-8d12-20f775429746" />
                <ParameterValue key_Name="XamlLoad" label="XamlLoad" weight="1" includeInGenerationType="1" resultWeight="1" comment="The tree will be constructed in xaml, which will be parsed and run in a dispatcher." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d4fe70a2-9abd-4aac-ba6e-86db6740c855" />
            </Parameter>
            <Parameter key_Name="TemplatedControlType" label="TemplatedControlType" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="85c3fba8-e6a1-4947-a416-f1431081b688">
                <ParameterValue key_Name="ContentControl" label="ContentControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="560d14a2-a1f5-47b1-8b00-9d9678ba3145" />
                <ParameterValue key_Name="ItemsControl" label="ItemsControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="6ce1ef66-8b2c-475c-942f-63b4b949683a" />
                <ParameterValue key_Name="CustomControl" label="CustomControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="6f256e3e-d023-446c-8b06-3a435140799e" />
            </Parameter>
            <Parameter key_Name="TemplateRootType" label="TemplateRootType" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09">
                <ParameterValue key_Name="Control" label="Control" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="1f9da78f-23db-4fb6-8067-b1cbe603a503" />
                <ParameterValue key_Name="Panel" label="Panel" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="266df06a-719b-44d3-b4f3-1e066a28c39f" />
                <ParameterValue key_Name="Decorator" label="Decorator" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="3d5f4ef5-4d7c-47e0-8c91-cdc47a9ee8c5" />
                <ParameterValue key_Name="FrameworkContentElement" label="FrameworkContentElement" weight="1" includeInGenerationType="0" resultWeight="1" comment="This is excluded because a FrameworkContentElement can be used as the root only when a template's target is also a FCE, but ControlTemplate works only on Controls." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="aa2fdc88-f3b4-4f94-a8bd-e2d4576fd7f3" />
                <ParameterValue key_Name="CustomPanel" label="CustomPanel" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="de35dab5-4d48-46a9-92b4-4efd68594035" />
                <ParameterValue key_Name="Viewport3D" label="Viewport3D" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="89083378-6bb9-4643-ad64-62944c67ef0c" />
                <ParameterValue key_Name="Grid" label="Grid" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="1eea8975-8ec7-4a7f-b7f6-6e30a20b0dd7" />
                <ParameterValue key_Name="CustomControl" label="CustomControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="e62a5741-7c43-46bc-9399-0f9f3cc4934f" />
            </Parameter>
            <Parameter key_Name="TemplateChildType" label="TemplateChildType" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe">
                <ParameterValue key_Name="FrameworkElement" label="FrameworkElement" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="013f9e88-dff4-4f78-bccc-14f256f99aa0" />
                <ParameterValue key_Name="FrameworkContentElement" label="FrameworkContentElement" weight="1" includeInGenerationType="1" resultWeight="1" comment="FrameworkContentElements cannot do binding." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" />
                <ParameterValue key_Name="DependencyObject" label="DependencyObject" weight="1" includeInGenerationType="0" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="a57db2d6-ad40-49d6-8f86-4f3e0b680ccc" />
                <ParameterValue key_Name="NonDependencyObject" label="NonDependencyObject" weight="1" includeInGenerationType="0" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="dc8da363-675f-4fb6-835d-085e380714dc" />
                <ParameterValue key_Name="Viewport3D" label="Viewport3D" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" />
                <ParameterValue key_Name="None" label="None" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" />
                <ParameterValue key_Name="IList" label="IList" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="7800aefa-8e14-4ef6-86a1-1f478092d901" />
                <ParameterValue key_Name="Shape" label="Shape" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d783cbed-007c-4c33-8cd6-be46807a4b05" />
                <ParameterValue key_Name="CustomControl" label="CustomControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="2779a54e-6b3a-49df-998b-a92989389ad5" />
            </Parameter>
            <Parameter key_Name="HasPropertyTrigger" label="HasPropertyTrigger" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="afd2a1ae-cbf7-4242-8723-c52c89724542">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="4940ad92-ef33-458e-b432-4051259f2452" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="40633ef4-4b1e-40f5-acd4-05a349b45f44" />
            </Parameter>
            <Parameter key_Name="HasEventTrigger" label="HasEventTrigger" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="f3592b40-997b-479a-aa6c-643e28be9a19">
                <ParameterValue key_Name="Loaded" label="Loaded" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d6e491f8-c4a2-4ce4-a669-c2582ec33f55" />
                <ParameterValue key_Name="PreviewMouseDown" label="PreviewMouseDown" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="ea844cef-0c7f-4c7b-8c1a-ed7cb8c22bd2" />
                <ParameterValue key_Name="None" label="None" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="8f63757c-4b03-492d-8b89-7374fb006f3b" />
            </Parameter>
            <Parameter key_Name="HasTemplateBind" label="HasTemplateBind" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="TemplatedParent TriggerTarget removes background property from templated parent so there is nothing to template bind to." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="4b9882e6-6e04-48c0-ad3f-6c3841442ac9" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="645be37f-0ada-495a-8bdb-52aa5b795b87" />
            </Parameter>
            <Parameter key_Name="DoesReferToStyle" label="DoesReferToStyle" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="e9036b04-fb98-4f0f-9e5b-a71c824f3106">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="0" resultWeight="1" comment="This restriction is due to bug 4." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="af904777-d248-477b-b4fe-f474a9f7e8ee" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="0aabc6c5-0d57-4c0a-9015-63c088f92f3e" />
            </Parameter>
            <Parameter key_Name="HasEventSet" label="HasEventSet" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="0c0d3990-8281-4951-b7b4-8510e9d80851">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="1eee55ed-5fa6-4afe-9449-c4b97bc28258" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d1c0caa1-6747-467e-a2ac-402c34ffba96" />
            </Parameter>
            <Parameter key_Name="IsKeySet" label="IsKeySet" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="fb093334-fb52-481d-b9d6-2bbfac744b91" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="This restriction is due to bug." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="60c11863-783d-44a8-83c3-402095e1a2b2" />
            </Parameter>
            <Parameter key_Name="TargetTypeSet" label="TargetTypeSet" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2">
                <ParameterValue key_Name="ContentControl" label="ContentControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="9ef96022-cda7-4695-a0d4-6f0c3d3642f8" />
                <ParameterValue key_Name="ItemsControl" label="ItemsControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="523f6074-5e49-4c5d-aea9-7287d06cd6f5" />
                <ParameterValue key_Name="CustomControl" label="CustomControl" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="addc269f-6ff2-4d99-9911-fe5259f701ae" />
            </Parameter>
            <Parameter key_Name="HowTemplateIsSet" label="HowTemplateIsSet" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142">
                <ParameterValue key_Name="DynamicResource" label="DynamicResource" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="c24bf90f-ccd4-47de-b7a0-812cea8169eb" />
                <ParameterValue key_Name="StaticResource" label="StaticResource" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="f908f384-b8cf-44f4-8443-cb270ffe3d68" />
                <ParameterValue key_Name="Inline" label="Inline" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="e4da3c86-722c-4900-98ff-29cb568e7cfb" />
                <ParameterValue key_Name="Implicit" label="Implicit" weight="1" includeInGenerationType="0" resultWeight="1" comment="This restriction is due to bug ." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="e159895a-79ee-442b-9682-bcdd8b241bc1" />
                <ParameterValue key_Name="Style" label="Style" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="5dfbd014-0f2b-497c-ad47-47a697e9946b" />
                <ParameterValue key_Name="InlineStyle" label="InlineStyle" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="6c25f758-5deb-4487-b42e-96f6c2cb8c3c" />
            </Parameter>
            <Parameter key_Name="DataSourceType" label="DataSourceType" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="d23e7747-ecb3-466b-8167-491ae2efc0f1">
                <ParameterValue key_Name="ObjectDataSource" label="ObjectDataSource" weight="1" includeInGenerationType="0" resultWeight="1" comment="TBD" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="7e1e5165-9042-4845-8914-f758abe922f0" />
                <ParameterValue key_Name="XmlDataSource" label="XmlDataSource" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="6951622f-baf5-43cb-ba9e-a4660c3a9a76" />
            </Parameter>
            <Parameter key_Name="TemplateType" label="TemplateType" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="bebd9f74-6773-4972-82ae-24befbab6617">
                <ParameterValue key_Name="ControlTemplate" label="ControlTemplate" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="b3c28511-ee87-421f-a4d4-0e8a2112a73f" />
                <ParameterValue key_Name="DataTemplate" label="DataTemplate" weight="1" includeInGenerationType="1" resultWeight="1" comment="Bug : Buttons don't work correctly with binding to a single element." automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="710e36c0-d9a9-43b2-b076-dd582392b405" />
            </Parameter>
            <Parameter key_Name="HasDataBinding" label="HasDataBinding" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="Does the templated element and elements in a DataTemplate reference a data source." dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="79d12f28-365a-4285-9630-642f50530c0d" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d9def757-ce7c-42eb-8f97-a16f38b58e7e" />
            </Parameter>
            <Parameter key_Name="TemplatePropertyName" label="TemplatePropertyName" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="5255e72a-30d8-4658-9b27-b4bcab3984da">
                <ParameterValue key_Name="Template" label="Template" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d197512d-9c67-40a2-913a-b498b01ea73b" />
                <ParameterValue key_Name="ContentTemplate" label="ContentTemplate" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="bce08a84-0325-409b-aa1b-c1383a42e93f" />
                <ParameterValue key_Name="ItemTemplate" label="ItemTemplate" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="1a2aaf92-992a-4599-8bba-465a9cd621fa" />
            </Parameter>
            <Parameter key_Name="TriggerTarget" label="TriggerTarget" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="Property triggers can target values of the template root, child or templated parent." dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad">
                <ParameterValue key_Name="TemplateRoot" label="TemplateRoot" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="c1f413a8-cf1e-4e98-bdc8-d6b0d128d770" />
                <ParameterValue key_Name="TemplatedParent" label="TemplatedParent" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="64bfd582-872a-4338-9d71-cfc40b705f09" />
                <ParameterValue key_Name="TemplateChild" label="TemplateChild" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="a3755416-7964-49a3-927f-97bef3adcb4d" />
            </Parameter>
            <Parameter key_Name="TriggerSource" label="TriggerSource" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="Property triggers can source off of the template root, child or the templated parent (default), " dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="00a5fc9a-23eb-481e-b6dd-2c7d8ce7ab5c">
                <ParameterValue key_Name="TemplatedParent" label="TemplatedParent" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="e2fcecf9-a6d9-4a21-be74-ad1ddf26e5ff" />
                <ParameterValue key_Name="TemplateRoot" label="TemplateRoot" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="e0c5f5f9-f06d-4528-bcfd-0f02283f8694" />
                <ParameterValue key_Name="TemplateChild" label="TemplateChild" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="d8a5ff3a-edc7-4466-b9f2-0cfdbcc166e5" />
            </Parameter>
            <Parameter key_Name="HasFreezableTriggerSetter" label="HasFreezableTriggerSetter" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1">
                <ParameterValue key_Name="True" label="True" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="2e0c2335-3fb7-4f8d-8a45-72ba32efcbbe" />
                <ParameterValue key_Name="False" label="False" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="7ba97df9-9f4a-4dca-a5b9-0bfbd9942ce4" />
            </Parameter>
            <Parameter key_Name="FreezableSetterValue" label="FreezableSetterValue" key_ActionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" comment="" dataType="" dataGenerationType="" dataGenerationValue="" includeInGeneration="true" includeInClusters="true" automated="false" isNumericInModel="false" inOutType="0" uiPersistentID="" hidden="false" persist="false" referenceID="30b86b82-39c5-4ad1-82f9-a12fc0b9ab4c">
                <ParameterValue key_Name="Local" label="Local" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="aa540947-7484-41f3-947f-e33873e439d8" />
                <ParameterValue key_Name="DynamicResource" label="DynamicResource" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="b1cb278a-3381-4b3c-9141-220dce7b9bd3" />
                <ParameterValue key_Name="StaticResource" label="StaticResource" weight="1" includeInGenerationType="1" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="13178efb-34eb-41b7-82b8-d57cf2d7b89a" />
                <ParameterValue key_Name="Binding" label="Binding" weight="1" includeInGenerationType="0" resultWeight="1" comment="" automated="false" valid="true" uiPersistentID="" mileStone="" referenceID="ac21fa95-dae8-46dc-92eb-a80594b618b0" />
            </Parameter>
        </Parameters>
        <ParameterCombinationRules seed="1" referenceID="6200bd92-f341-4c12-96ab-67b36e9f9a7d">
            <ParameterCluster key_Name="FullGroup" label="FullGroup" comment="" combinatoryType="3" include="true" combinatoryOrder="2" referenceID="482ab4c5-fcee-467f-9c3c-01b18eb49d25">
                <ParameterClusterElement key_ParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" key_ModelReferenceID="" referenceID="85292eb0-4267-4288-9586-e7689c3fa3ca" />
                <ParameterClusterElement key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ModelReferenceID="" referenceID="540c3365-77cb-48ff-84e0-95444bfd9085" />
                <ParameterClusterElement key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ModelReferenceID="" referenceID="09ef502e-411b-4c1d-9f95-d17941b69040" />
                <ParameterClusterElement key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ModelReferenceID="" referenceID="e70b32c2-c17b-433a-9f3f-854ff40901b0" />
                <ParameterClusterElement key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ModelReferenceID="" referenceID="9cb13a1b-f75e-4213-9652-1fdb2aa9ff20" />
                <ParameterClusterElement key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ModelReferenceID="" referenceID="7da753d7-497c-4480-8a8e-237850841246" />
                <ParameterClusterElement key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ModelReferenceID="" referenceID="8d271dff-5e15-4dd4-9ac9-a01a219e71dc" />
                <ParameterClusterElement key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ModelReferenceID="" referenceID="55b6c6e9-ed69-4a24-af25-467ace626a94" />
                <ParameterClusterElement key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ModelReferenceID="" referenceID="c3544ce4-7901-4579-97b4-600994785116" />
                <ParameterClusterElement key_ParameterReferenceID="afd2a1ae-cbf7-4242-8723-c52c89724542" key_ModelReferenceID="" referenceID="f16dc03d-979f-4b9d-9a52-cb139ec22136" />
                <ParameterClusterElement key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ModelReferenceID="" referenceID="1ca53e93-8172-4040-ada7-9e4f30cce14c" />
                <ParameterClusterElement key_ParameterReferenceID="00a5fc9a-23eb-481e-b6dd-2c7d8ce7ab5c" key_ModelReferenceID="" referenceID="6d834ca9-e9b5-4915-ba93-cfa3207628c3" />
                <ParameterClusterElement key_ParameterReferenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1" key_ModelReferenceID="" referenceID="7960e4cd-9115-4936-956a-a35a47e837e1" />
                <ParameterClusterElement key_ParameterReferenceID="30b86b82-39c5-4ad1-82f9-a12fc0b9ab4c" key_ModelReferenceID="" referenceID="ca817d4f-3733-4f59-a03d-c4adc4fbdbf4" />
                <ParameterClusterElement key_ParameterReferenceID="f3592b40-997b-479a-aa6c-643e28be9a19" key_ModelReferenceID="" referenceID="19cf46ca-51f8-4a47-8249-bcac7465b76c" />
                <ParameterClusterElement key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ModelReferenceID="" referenceID="2b1fcd41-3c96-4161-93fb-2c39dc73b275" />
                <ParameterClusterElement key_ParameterReferenceID="e9036b04-fb98-4f0f-9e5b-a71c824f3106" key_ModelReferenceID="" referenceID="05e11c4f-7bba-422d-8675-275ea362dde4" />
                <ParameterClusterElement key_ParameterReferenceID="0c0d3990-8281-4951-b7b4-8510e9d80851" key_ModelReferenceID="" referenceID="103468c2-180c-4c11-bfba-4fcf32bd6b87" />
                <ParameterClusterElement key_ParameterReferenceID="d23e7747-ecb3-466b-8167-491ae2efc0f1" key_ModelReferenceID="" referenceID="77de274e-0a2d-415b-8e83-b5dacf5e3676" />
                <ParameterClusterElement key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ModelReferenceID="" referenceID="f8a31d16-adaf-4097-b13f-74361c6fb2f0" />
            </ParameterCluster>
            <ParameterCluster key_Name="N-Tuple" label="N-Tuple" comment="" combinatoryType="3" include="false" combinatoryOrder="3" referenceID="c2c1895b-663f-482d-83ed-a11ea8aa1851">
                <ParameterClusterElement key_ParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" key_ModelReferenceID="" referenceID="1124ae68-3517-4c4b-a86a-ae88c2d68cad" />
                <ParameterClusterElement key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ModelReferenceID="" referenceID="04100d74-34e4-4e8d-b8cf-21ad8d18049a" />
                <ParameterClusterElement key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ModelReferenceID="" referenceID="0a012dcf-3440-4477-8c45-7d0b39017b59" />
                <ParameterClusterElement key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ModelReferenceID="" referenceID="4593d5ae-340f-4f2d-a2a0-1d6a3424eae0" />
                <ParameterClusterElement key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ModelReferenceID="" referenceID="c56445cb-f318-4178-9642-e430c7ccdd41" />
                <ParameterClusterElement key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ModelReferenceID="" referenceID="17c7d3e7-3822-4592-b8d7-3952698991e9" />
                <ParameterClusterElement key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ModelReferenceID="" referenceID="508c8e43-2ed8-47f1-a17a-814ad8b08be4" />
                <ParameterClusterElement key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ModelReferenceID="" referenceID="d4a29de2-b061-4a3e-bc18-924fbcf4ccc8" />
                <ParameterClusterElement key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ModelReferenceID="" referenceID="9908940e-610f-4ce9-8c1f-6058f1a4abe2" />
                <ParameterClusterElement key_ParameterReferenceID="afd2a1ae-cbf7-4242-8723-c52c89724542" key_ModelReferenceID="" referenceID="55081589-9fd7-4a38-986f-18708d9443e3" />
                <ParameterClusterElement key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ModelReferenceID="" referenceID="84fb2166-3230-4a1c-8235-86472db81e13" />
                <ParameterClusterElement key_ParameterReferenceID="00a5fc9a-23eb-481e-b6dd-2c7d8ce7ab5c" key_ModelReferenceID="" referenceID="9bf29f4f-482a-4a60-a105-61699f680084" />
                <ParameterClusterElement key_ParameterReferenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1" key_ModelReferenceID="" referenceID="16e03995-d485-46cf-9533-a3d71c5b2a4f" />
                <ParameterClusterElement key_ParameterReferenceID="30b86b82-39c5-4ad1-82f9-a12fc0b9ab4c" key_ModelReferenceID="" referenceID="1cc8a93a-5bf6-4d17-b12c-39e5597f6888" />
                <ParameterClusterElement key_ParameterReferenceID="f3592b40-997b-479a-aa6c-643e28be9a19" key_ModelReferenceID="" referenceID="012b526f-d6d2-45bc-b8c4-8706acadb142" />
                <ParameterClusterElement key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ModelReferenceID="" referenceID="7a6a4c89-e894-46d2-8f9a-8d4079d9c9b3" />
                <ParameterClusterElement key_ParameterReferenceID="e9036b04-fb98-4f0f-9e5b-a71c824f3106" key_ModelReferenceID="" referenceID="ff6da70b-23f2-48af-b596-f0684092cdc4" />
                <ParameterClusterElement key_ParameterReferenceID="0c0d3990-8281-4951-b7b4-8510e9d80851" key_ModelReferenceID="" referenceID="c1f86a43-b869-4f18-a5b4-b278e1255ad0" />
                <ParameterClusterElement key_ParameterReferenceID="d23e7747-ecb3-466b-8167-491ae2efc0f1" key_ModelReferenceID="" referenceID="b0af9dec-1160-446b-b374-a75e03785382" />
                <ParameterClusterElement key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ModelReferenceID="" referenceID="045b25bd-12e3-43e0-ad87-ce6ff7ac3dd8" />
            </ParameterCluster>
            <ParameterDependencyGroup key_Name="RestrictTargetType" label="RestrictTargetType" comment="" include="true" referenceID="81f8261f-0229-4a3c-bf4b-4a4057142f10">
                <ParameterDependencyElement criteria="0" referenceID="ba906de9-5317-42cf-9086-e6bd93b16b32">
                    <ParameterDependencyCondition referenceID="f9cf09de-503e-4d68-8678-3db63a2d4534">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6ce1ef66-8b2c-475c-942f-63b4b949683a" key_ModelReferenceID="" referenceID="48682f08-bccc-4f1a-8c56-fd82518845d7" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d59b86c1-77d3-455a-ae44-771f6ebce541">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="9ef96022-cda7-4695-a0d4-6f0c3d3642f8" key_ModelReferenceID="" referenceID="6416ccfa-e19c-451a-87e7-3647577121a1" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="2296a6b0-22d4-4680-bbcb-f46a11b9cb59">
                    <ParameterDependencyCondition referenceID="1eaeca55-2bbb-41a8-b295-5f6706b9e466">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="a77d2cda-acb9-4bff-97bf-05c512821b4b" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="76aac4be-a0f6-486b-a764-963ce274a831">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="9ef96022-cda7-4695-a0d4-6f0c3d3642f8" key_ModelReferenceID="" referenceID="cb3af984-7f84-4601-9bc9-e1e83e731ea0" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="a9014824-a2e2-4ff1-a984-f94c51862fe5">
                    <ParameterDependencyCondition referenceID="66f03f1c-8d8f-420a-b3a5-fb0d91e1790d">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="560d14a2-a1f5-47b1-8b00-9d9678ba3145" key_ModelReferenceID="" referenceID="770c6fbb-3fbb-4a26-9e60-26f156424804" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="25c909f2-8173-4bf0-bda9-415e72097618">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="523f6074-5e49-4c5d-aea9-7287d06cd6f5" key_ModelReferenceID="" referenceID="a38834aa-044c-4801-a169-7ddea8adc597" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="0a2b77a1-d8dd-4361-98da-f525b7aaf1e0">
                    <ParameterDependencyCondition referenceID="343e4a3b-fac4-4b18-8f12-2005fa96e9db">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="560d14a2-a1f5-47b1-8b00-9d9678ba3145" key_ModelReferenceID="" referenceID="770c6fbb-3fbb-4a26-9e60-26f156424804" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="37b89052-d913-44a9-899d-9312414fdfc9">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="addc269f-6ff2-4d99-9911-fe5259f701ae" key_ModelReferenceID="" referenceID="ccfda806-1897-429b-99b9-cd84473303fb" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="dad71c3f-d7d5-4fcc-93a9-839c975e6934">
                    <ParameterDependencyCondition referenceID="2488119e-8e14-4f1c-b112-5e929b7d41da">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6ce1ef66-8b2c-475c-942f-63b4b949683a" key_ModelReferenceID="" referenceID="48682f08-bccc-4f1a-8c56-fd82518845d7" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="c594bb58-ab6c-4099-b0b2-058f23ca0596">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="addc269f-6ff2-4d99-9911-fe5259f701ae" key_ModelReferenceID="" referenceID="cd203606-9a37-4017-a1a8-48263f9997f6" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="09d43f72-867b-4618-ae4d-9fc06d5cb452">
                    <ParameterDependencyCondition referenceID="c318b54d-ba00-482c-b344-30fe983673e4">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="a77d2cda-acb9-4bff-97bf-05c512821b4b" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="959d4021-3a39-4cd9-be65-7567be101251">
                        <ParameterDependencyTermValue key_ParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" key_ParameterValueReferenceID="523f6074-5e49-4c5d-aea9-7287d06cd6f5" key_ModelReferenceID="" referenceID="43d6d2d1-8eea-4dac-9b29-bf4f3caf031f" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictIsKeySet" label="RestrictIsKeySet" comment="This restriction is to work around bug ." include="true" referenceID="f8084e79-c3cd-4e71-baf9-c5acfb56c01d">
                <ParameterDependencyElement criteria="0" referenceID="4d6c0602-8322-44ab-93eb-553ccf237139">
                    <ParameterDependencyCondition referenceID="950ddc47-258e-4436-b901-6bec9686e564">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="e4da3c86-722c-4900-98ff-29cb568e7cfb" key_ModelReferenceID="" referenceID="00434901-819e-4496-b61c-b55baa9d7986" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="31694aa8-fefe-497c-b32a-645f59738e97">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="fb093334-fb52-481d-b9d6-2bbfac744b91" key_ModelReferenceID="" referenceID="f4d31c7a-6466-4d54-992e-82823b0b0d1a" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="aafe51f9-285d-42ff-8062-9ee9ab434d43">
                    <ParameterDependencyCondition referenceID="7d6613b4-0f71-4055-9179-c2da639635a5">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="e159895a-79ee-442b-9682-bcdd8b241bc1" key_ModelReferenceID="" referenceID="bdbc2a5a-a862-4d58-83c0-4fcf9bb62489" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="31bdbd2a-e141-47ef-80bc-7ed7f30f2642">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="fb093334-fb52-481d-b9d6-2bbfac744b91" key_ModelReferenceID="" referenceID="2c515060-b2fd-48ea-9f4b-64b659be74b7" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="5e24aa50-5b5e-4898-b56e-2d7bc7827dea">
                    <ParameterDependencyCondition referenceID="63f51ce6-d27c-4229-a439-95442f76cb20">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="5dfbd014-0f2b-497c-ad47-47a697e9946b" key_ModelReferenceID="" referenceID="4ccd9dc4-fd54-456f-bcd6-00b39370d6a1" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="176fa6a4-48e2-4768-b474-ff9440fa28ec">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="fb093334-fb52-481d-b9d6-2bbfac744b91" key_ModelReferenceID="" referenceID="6513d5dd-df9d-44b5-8ae8-9527c43723bf" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="13fab3d7-2dc9-4ea1-b4a5-4e1c79e3655d">
                    <ParameterDependencyCondition referenceID="f7cc398f-b486-4250-9412-d3d3b230251f">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="c24bf90f-ccd4-47de-b7a0-812cea8169eb" key_ModelReferenceID="" referenceID="d54481a5-f74c-4a1d-8a48-446c1742c4e4" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="13d6d3db-8640-4b9c-a0fb-4ebcd4c06701">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="60c11863-783d-44a8-83c3-402095e1a2b2" key_ModelReferenceID="" referenceID="bdde9e79-a4b6-4572-8a41-47e524ce2daa" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="88a0d7b7-b517-4ab7-afb1-ae2ea7f2f240">
                    <ParameterDependencyCondition referenceID="f137cb0b-64af-4224-9a84-715632d97cc0">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="f908f384-b8cf-44f4-8443-cb270ffe3d68" key_ModelReferenceID="" referenceID="067a112d-cdd0-4400-9661-361b0960b551" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="6f00bac2-b61f-4767-9c2b-d54451ca35be">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="60c11863-783d-44a8-83c3-402095e1a2b2" key_ModelReferenceID="" referenceID="f3e046f3-548f-41d6-95c4-af0fea8b3678" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="5a798b82-cf5c-442b-ab1c-b74176da588b">
                    <ParameterDependencyCondition referenceID="3235b2aa-acad-49a0-9f57-ceecb2725b24">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" key_ParameterValueReferenceID="6c25f758-5deb-4487-b42e-96f6c2cb8c3c" key_ModelReferenceID="" referenceID="49500660-1a63-4050-beb8-f4fa609b839f" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="20b31276-31d3-4571-b0c3-b42d6cf6ce26">
                        <ParameterDependencyTermValue key_ParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" key_ParameterValueReferenceID="fb093334-fb52-481d-b9d6-2bbfac744b91" key_ModelReferenceID="" referenceID="88e236f3-9805-4887-bffb-093222744f37" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictTemplateChildType" label="RestrictTemplateChildType" comment="A FrameworkContentElement is valid only under a content host.  It works in Button because its visual tree contains a content host element." include="true" referenceID="8222d1ab-e53e-46b0-b40d-bc4b208397d9">
                <ParameterDependencyElement criteria="0" referenceID="fc44b666-3b4d-4372-95c7-8edc632adebb">
                    <ParameterDependencyCondition referenceID="8b2264e2-432b-48f2-b2a2-5c0b7b01a99a">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="3d5f4ef5-4d7c-47e0-8c91-cdc47a9ee8c5" key_ModelReferenceID="" referenceID="352bb439-4237-4a83-8714-f68af4ed4bb3" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="3c78d8c4-05f7-4b9c-b0ad-9c146a8f21a1">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="d8cb197c-ecb8-4f74-901e-1432cd9906c4" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="da16fd43-9adf-4de1-95ff-67629c86bccc">
                    <ParameterDependencyCondition referenceID="bf1ce31b-f17e-4d61-8563-1fbfab55057c">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="266df06a-719b-44d3-b4f3-1e066a28c39f" key_ModelReferenceID="" referenceID="a127ffee-2a74-4e18-9b43-a608b2f9c52f" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="082868c9-274e-4f84-8e1b-b73897b83992">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="9cc16fae-8979-477d-8cea-2dab75747b57" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="3daa1fc1-3648-4e8e-8a04-97aaff84ff92">
                    <ParameterDependencyCondition referenceID="50160374-c81e-4039-9da2-f4548f33e2ff">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="de35dab5-4d48-46a9-92b4-4efd68594035" key_ModelReferenceID="" referenceID="e7b4ef32-d86c-4314-a835-6b293230804a" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="77403620-0fbb-4efc-b4e8-43fa15785e5e">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="a11788cf-6647-4776-af81-c67dfd05852d" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="4b7b0ebc-fd43-4136-9538-550363c25264">
                    <ParameterDependencyCondition referenceID="a56b857c-7eff-40a3-862e-3e9388b35097">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="0ff7652c-386b-4dd7-8555-a116712cc6b7">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="013f9e88-dff4-4f78-bccc-14f256f99aa0" key_ModelReferenceID="" referenceID="eb1a99dc-c32a-4bb7-bdc3-5adb1461c23e" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="909f8bd3-f133-49cb-8ad2-ec5fb6a1e1a7">
                    <ParameterDependencyCondition referenceID="02d384d7-1998-44c5-9a82-e55294da51db">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="f3d55678-98de-4e9c-8b7c-2ff7bf76f1d0">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="946e6396-3c23-40b7-b338-f4d44e817db3" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="f9161e43-bc2a-4e96-b88c-87e3d7584d97">
                    <ParameterDependencyCondition referenceID="1f8e06df-0cd3-40d5-814c-f3c6a6897b69">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="c2b59406-b2b5-4a54-a4ba-a541aac77bb1">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="a57db2d6-ad40-49d6-8f86-4f3e0b680ccc" key_ModelReferenceID="" referenceID="5b8e38bc-d277-48db-bb91-475c5cffd307" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="8f59f58c-7924-42d1-834e-a70a2a0a3015">
                    <ParameterDependencyCondition referenceID="8e81d2e5-de09-4053-bc1c-8af21c39118b">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d0abebda-198d-4fcf-9aee-34b6fc4c0608">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="dc8da363-675f-4fb6-835d-085e380714dc" key_ModelReferenceID="" referenceID="0ea3a95a-c273-40d4-90a9-75e1ec6b0cc6" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="6fba206b-735c-47a5-8165-903d48098d02">
                    <ParameterDependencyCondition referenceID="b794e8c1-b5b0-4605-93f8-48c0368adbb0">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="6a562a2c-f6d2-4839-94ac-55c3e0530eac">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" key_ModelReferenceID="" referenceID="d5c439ca-5727-441a-ae93-3f687e1c393a" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="944da9cf-9f61-450d-b510-6e4d4a07d73f">
                    <ParameterDependencyCondition referenceID="29e6113e-8bf2-471b-b0ef-9ab0af95d269">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="1f9da78f-23db-4fb6-8067-b1cbe603a503" key_ModelReferenceID="" referenceID="cccc82d8-4e93-4782-af31-2a24764b68a1" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="14538f22-de56-4e7a-8c71-ebffc31db110">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="704b1595-c2ab-41e3-994d-af5fefbe0491" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="5d120b33-47f3-44f4-bb23-06ee6f6903e5">
                    <ParameterDependencyCondition referenceID="683d233f-84cf-4dd7-a872-69108411cf24">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="266df06a-719b-44d3-b4f3-1e066a28c39f" key_ModelReferenceID="" referenceID="a127ffee-2a74-4e18-9b43-a608b2f9c52f" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="6bd2e2c4-aac9-4e82-8e5c-d30cc0ac7f78">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="eb9bf56d-5e0d-41c0-b4e1-e03f20f03b23" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="3bad57fc-3294-4c96-8f07-1fee4b20a1a9">
                    <ParameterDependencyCondition referenceID="c3da400e-5c72-4be2-874d-4ba64cc1bb0c">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="3d5f4ef5-4d7c-47e0-8c91-cdc47a9ee8c5" key_ModelReferenceID="" referenceID="352bb439-4237-4a83-8714-f68af4ed4bb3" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="e6b0caab-c70b-4d14-b695-de3797e36c97">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="055aa3d1-914c-475e-8ebd-ef7107606b45" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="abea3702-60fe-4924-b76f-6b8b9d65f697">
                    <ParameterDependencyCondition referenceID="1962161e-4ee2-4af8-946b-954e70b6e6c2">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="de35dab5-4d48-46a9-92b4-4efd68594035" key_ModelReferenceID="" referenceID="e7b4ef32-d86c-4314-a835-6b293230804a" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="1a78f1e3-8e0e-466b-b677-d29d40917480">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="b5c16625-c433-40d9-86c1-d0c1ee1fc26c" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="4cafc964-4ba5-46f7-b55e-385e0e0a3498">
                    <ParameterDependencyCondition referenceID="81ff1749-3e88-4278-a36e-f7ed55eebf5f">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="3950505e-2fcb-4dc1-a901-cf7d1d1fe960">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="7800aefa-8e14-4ef6-86a1-1f478092d901" key_ModelReferenceID="" referenceID="d59d5e39-7f0f-4290-9865-62d281595ca3" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="532993df-744d-465f-9771-0d2b056b3d44">
                    <ParameterDependencyCondition referenceID="4a799af4-6896-4024-8ce3-6710d294c923">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="1eea8975-8ec7-4a7f-b7f6-6e30a20b0dd7" key_ModelReferenceID="" referenceID="1e56cdcd-6b00-481d-af58-9aff8e14bd6c" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="46ec6fbc-7228-40d0-a498-00708744e7e2">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="63d2e89f-0f7d-4adc-85ad-8dfced289b3a" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="42cc4311-0c6f-4b0b-9631-9f917de48957">
                    <ParameterDependencyCondition referenceID="d21f6551-e2a0-42c7-b577-8b759b38a6d8">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="1eea8975-8ec7-4a7f-b7f6-6e30a20b0dd7" key_ModelReferenceID="" referenceID="1e56cdcd-6b00-481d-af58-9aff8e14bd6c" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="5f530d6e-7159-4b13-b8b0-332b148623ef">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="45c3dc25-6059-4239-bfff-c98bc6aaf58e" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="9878c287-1aa7-42c7-b7fb-0e5ffeba2358">
                    <ParameterDependencyCondition referenceID="dfecbb73-bb42-4b7a-a531-bdda164feced">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="dd22f0b6-ece7-43a5-98f6-fc2ebd1620af">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="d783cbed-007c-4c33-8cd6-be46807a4b05" key_ModelReferenceID="" referenceID="0b5c1110-0f06-420e-8391-1d3ceab73258" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="dc0d3f14-043e-4b69-bd00-7d62bfb65ea5">
                    <ParameterDependencyCondition referenceID="a98074b9-9dc4-43c1-99b3-c5ab21f6a04b">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="cfa18d11-eceb-4a33-94bc-fb3a97f2c107" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="61fac292-0187-4eca-8499-40c5aff05f2e">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="2779a54e-6b3a-49df-998b-a92989389ad5" key_ModelReferenceID="" referenceID="a4f961c5-ef30-496f-bec2-63a78ad6c646" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="b99fd89a-7058-4a6b-8360-025b174c9939">
                    <ParameterDependencyCondition referenceID="60fb48b7-74a1-4c92-b2fc-10b40c66225f">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="e62a5741-7c43-46bc-9399-0f9f3cc4934f" key_ModelReferenceID="" referenceID="d4feda29-da56-4027-a893-d319b33c47a5" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d0d7f7e8-b9b5-4674-8f8c-fcf730077ff5">
                        <ParameterDependencyTermValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="087d7f38-2b73-433b-822b-bc8f9b03e2d7" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictHasEventSet" label="RestrictHasEventSet" comment="Don't set an event handler for XamlLoad (non-compile) tests." include="true" referenceID="ef6b8d83-5315-4845-8fb2-9bed634a3e82">
                <ParameterDependencyElement criteria="0" referenceID="71bea57d-38c7-4b52-b7d8-08020acfee0d">
                    <ParameterDependencyCondition referenceID="ef6d4957-bd0a-4edc-a234-2c321c4eeecd">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" key_ParameterValueReferenceID="d4fe70a2-9abd-4aac-ba6e-86db6740c855" key_ModelReferenceID="" referenceID="3470e7a5-8477-4e5a-87fc-cbe7b902f790" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="9e6059d3-dbd8-4f75-ac40-88d90807c564">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0c0d3990-8281-4951-b7b4-8510e9d80851" key_ParameterValueReferenceID="1eee55ed-5fa6-4afe-9449-c4b97bc28258" key_ModelReferenceID="" referenceID="0ad7ee92-f2bc-4dd0-a043-297ba55ccf72" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictTemplatedControlType" label="RestrictTemplatedControlType" comment="" include="false" referenceID="a5b720da-9dc7-479f-837d-f10422d358af">
                <ParameterDependencyElement criteria="0" referenceID="2f9af551-c26f-4d98-ba0e-53bf5856ce6c">
                    <ParameterDependencyCondition referenceID="42ad0b53-a917-4520-8815-3f89ed4cfd7a">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="710e36c0-d9a9-43b2-b076-dd582392b405" key_ModelReferenceID="" referenceID="2e611cef-cb28-4910-8143-e9327a2897c7" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="3a8ea021-16cf-44fb-953c-80e93187e4e6">
                        <ParameterDependencyTermValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="a9165e1e-8187-4d38-a182-340101acf4d6" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictHasTemplateBind" label="RestrictHasTemplateBind" comment="" include="true" referenceID="242dd5d0-3f58-4e69-966c-470e48adfed6">
                <ParameterDependencyElement criteria="0" referenceID="ce50b89c-0ed1-421a-a0ee-0d535a724782">
                    <ParameterDependencyCondition referenceID="a970bbe2-e5c9-4589-818e-c289d162af61">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="710e36c0-d9a9-43b2-b076-dd582392b405" key_ModelReferenceID="" referenceID="378fe907-05fc-43f2-b30a-888366ec4170" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="374e5c11-2b6d-49ff-8132-f22260a2298d">
                        <ParameterDependencyTermValue key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ParameterValueReferenceID="4b9882e6-6e04-48c0-ad3f-6c3841442ac9" key_ModelReferenceID="" referenceID="6991e3a0-77d2-48b1-b167-d5f3d1b7fef7" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="3b176a73-9252-4f49-847a-9dbc6a66b856">
                    <ParameterDependencyCondition referenceID="82a92eb7-0955-4f78-a010-b80b4457c89c">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" key_ModelReferenceID="" referenceID="58896b49-6a7d-43a8-a22c-6f289e2e3b4a" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="e1a128b4-a9fa-4b1c-a70c-89199bdcb544">
                        <ParameterDependencyTermValue key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ParameterValueReferenceID="4b9882e6-6e04-48c0-ad3f-6c3841442ac9" key_ModelReferenceID="" referenceID="e7602d73-34de-4770-9def-40c4900e6261" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="05c84049-85b0-44a2-ac4e-ab26eb7b6c08">
                    <ParameterDependencyCondition referenceID="3b30f746-01a2-46a6-88fa-a2bdbc615956">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="080dc9be-cfc9-4a98-b467-3d1a7c2a7950" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="690489a7-4ee2-4dee-8004-88ee70a42dc9">
                        <ParameterDependencyTermValue key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ParameterValueReferenceID="4b9882e6-6e04-48c0-ad3f-6c3841442ac9" key_ModelReferenceID="" referenceID="ce5b1aa9-77df-4bb4-a1d5-dc2059134e07" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictHasDataBinding" label="RestrictHasDataBinding" comment="Only do data binding for data templates, but it's not required. Also, data binding isn't possible from a content element." include="true" referenceID="4b43e4fc-1e44-4d7d-865a-4cd9ab54d2eb">
                <ParameterDependencyElement criteria="0" referenceID="51276696-65bc-42b0-ad4e-17d1f57df149">
                    <ParameterDependencyCondition referenceID="0bffa2b8-865d-4d82-9d22-f4659a91ca18">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="b3c28511-ee87-421f-a4d4-0e8a2112a73f" key_ModelReferenceID="" referenceID="9e3ecc8b-6d34-4abe-9bcb-a6f28d572f20" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="be8a0885-13c3-49d6-a526-c54d488cf6ee">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="ff0ade7b-97c2-40c8-b1f8-9ce49745c5b3" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="3504b59f-846d-48fb-9615-c64feb5df42b">
                    <ParameterDependencyCondition referenceID="bf4a05b0-756f-4e09-9952-6fee889281b9">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="46f12205-6f20-4f7c-b3a1-6ff7a548badf" key_ModelReferenceID="" referenceID="132434ff-76b4-4061-b46e-86ddfd81bb00" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="a2394c9a-3971-409d-ad7c-baaf1fd57ad6">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="79c0c6c6-3b63-4e12-aac7-579c12efc17a" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="b9f992b5-eaff-4c43-b6cf-790bb024be9b">
                    <ParameterDependencyCondition referenceID="e70f704d-1b58-44fb-8626-f4199a267054">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="dab5ed86-bfc8-4dc2-b0d1-551efc980323" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="dc325262-5658-4ff9-bbc9-599c05139c40">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="c9d6d1f9-5316-4a14-a75e-c30f9de9478b" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="b9378951-9911-4901-bc04-5224572cdad4">
                    <ParameterDependencyCondition referenceID="3ba2735e-cdf9-4f00-b28d-1fb41c864799">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" key_ModelReferenceID="" referenceID="56dec813-c522-4140-ac0a-9fcb4d28682f" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="4ea8bc04-70e5-4c56-9d11-965e04d1808c">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="6031e7fa-3c65-4204-b5f2-665ebd51016a" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="f0db2a9a-3c7f-436e-8a68-3f842f38481e">
                    <ParameterDependencyCondition referenceID="37137166-9cbc-4de2-9524-05067ff7ed98">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="7800aefa-8e14-4ef6-86a1-1f478092d901" key_ModelReferenceID="" referenceID="dd024035-8f39-436e-a6d5-5912e8f4bb49" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="65eb1298-199c-44f5-bb6a-6f76c23b18fe">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="411c2126-1b17-48e8-b195-80d63b392774" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="bc684143-4b52-478b-85d8-154c4c3327fa">
                    <ParameterDependencyCondition referenceID="c4b84d73-782f-4e72-97d7-d0f5e9817a55">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="d783cbed-007c-4c33-8cd6-be46807a4b05" key_ModelReferenceID="" referenceID="bffa683f-8f9d-4e25-aa5e-9579f8d1a1f2" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="bece6db5-bec7-46dd-a316-8530a533cdb7">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="18612962-594e-496c-95e5-f46432470fd6" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="b5e8cb80-449b-4870-bd0d-2e3e60dfd4a2">
                    <ParameterDependencyCondition referenceID="402efaf4-70d0-408e-8d1a-c9e5c8e719a5">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="2779a54e-6b3a-49df-998b-a92989389ad5" key_ModelReferenceID="" referenceID="a1e0c6ab-b97e-4115-a078-20e69c9b4e37" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="9139f33d-dbcb-49ea-b515-aa41541946f5">
                        <ParameterDependencyTermValue key_ParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" key_ParameterValueReferenceID="79d12f28-365a-4285-9630-642f50530c0d" key_ModelReferenceID="" referenceID="9d61b61a-d1e1-44ca-8e68-8b1c65e27edb" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictTemplatePropertyName" label="RestrictTemplatePropertyName" comment="The property name referencing a template depends on the item being templated and the type of template." include="true" referenceID="b4c8ad33-d17e-4a89-a5b7-38ad87f48bfd">
                <ParameterDependencyElement criteria="0" referenceID="cf691f64-c9d2-4bd6-9f2e-fd2649287f96">
                    <ParameterDependencyCondition referenceID="cdc5cfd2-3332-4909-9a10-7860f2eb3eba">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="b3c28511-ee87-421f-a4d4-0e8a2112a73f" key_ModelReferenceID="" referenceID="f0602db0-9348-4717-9556-21ff9b1c73b4" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="3a2aa7cc-0dac-45bc-b903-58b8f3c9b8be">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="bce08a84-0325-409b-aa1b-c1383a42e93f" key_ModelReferenceID="" referenceID="db8b7f54-b2b6-421a-b8b7-c3bd525c05d0" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="d853e801-51f1-49b3-bc49-155e4b05f670">
                    <ParameterDependencyCondition referenceID="ac1271f6-1ecc-4858-9439-c8482dcd4230">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="710e36c0-d9a9-43b2-b076-dd582392b405" key_ModelReferenceID="" referenceID="1258dce3-22e8-43f8-890e-4ddc87c94f3e" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="1a76e7e8-676a-401c-997c-b38f4e5ddce0">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="d197512d-9c67-40a2-913a-b498b01ea73b" key_ModelReferenceID="" referenceID="ca78c3d4-da98-465b-969d-2e826ed99d77" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="c0777a6e-9108-4e82-b26e-67f8d5e9fd63">
                    <ParameterDependencyCondition referenceID="831b3a28-7f71-4654-9d3d-eef9d127b230">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6ce1ef66-8b2c-475c-942f-63b4b949683a" key_ModelReferenceID="" referenceID="debb6b8f-45e1-48f2-a85f-000bd92125ac" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="e9d63827-c3d2-4c5f-abfb-6a60d645b137">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="bce08a84-0325-409b-aa1b-c1383a42e93f" key_ModelReferenceID="" referenceID="11438c98-da75-4162-9255-a611e74851a9" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="2eb7e1f1-4ff0-4955-809f-23c8b0975bd5">
                    <ParameterDependencyCondition referenceID="2a07bb69-9a25-4bea-a434-9b778ee50388">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="b3c28511-ee87-421f-a4d4-0e8a2112a73f" key_ModelReferenceID="" referenceID="f0602db0-9348-4717-9556-21ff9b1c73b4" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="edafa32b-ee2a-48be-96c0-1fab8ad7af01">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="1a2aaf92-992a-4599-8bba-465a9cd621fa" key_ModelReferenceID="" referenceID="0aaa59d6-0db9-45ea-92f1-913f6f33baaa" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="efd69e22-7c57-4ed8-9b23-faa43573f4e4">
                    <ParameterDependencyCondition referenceID="534dfbbd-95c8-4834-b87d-7a819b28448c">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="93630674-41fd-4bd4-a299-911e48797148" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="f826df0f-f6f2-43f5-abc9-a08698f1301d">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="1a2aaf92-992a-4599-8bba-465a9cd621fa" key_ModelReferenceID="" referenceID="903ba52b-1408-4f4d-a56f-16006d38f26d" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="7f8ec932-288d-463b-8a91-6787543e013a">
                    <ParameterDependencyCondition referenceID="7f3a9f34-e479-4672-858b-158147981d2c">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="560d14a2-a1f5-47b1-8b00-9d9678ba3145" key_ModelReferenceID="" referenceID="79c164b1-7405-47af-9a49-947f2e7dba0f" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="7da537a3-af3b-4d17-841e-39fcfdc59bf6">
                        <ParameterDependencyTermValue key_ParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" key_ParameterValueReferenceID="1a2aaf92-992a-4599-8bba-465a9cd621fa" key_ModelReferenceID="" referenceID="175dc7f7-9c83-4c1a-a0b1-f9bda215ca6d" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictDataTemplate" label="RestrictDataTemplate" comment="DataTemplate's do not serialize correctly. Sam will check in fix shortly." include="true" referenceID="cf1561b6-63ff-4878-b513-56fc7f9be322">
                <ParameterDependencyElement criteria="0" referenceID="1a840128-76e4-4e17-ac84-86334cc80574">
                    <ParameterDependencyCondition referenceID="362190a7-207a-48d6-b9cf-40dc34a012ee">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" key_ParameterValueReferenceID="d4fe70a2-9abd-4aac-ba6e-86db6740c855" key_ModelReferenceID="" referenceID="387a04b5-1bb1-4c45-ac41-43db5d45cd0b" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d6ef92a0-5bd5-4e20-a345-509e55e35414">
                        <ParameterDependencyTermValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="710e36c0-d9a9-43b2-b076-dd582392b405" key_ModelReferenceID="" referenceID="adb11a07-8ac9-4a6b-9e0e-31c27317dae3" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="ba47e926-f7da-4a50-bde1-f3549e895def">
                    <ParameterDependencyCondition referenceID="c3f37ef6-9a36-4b99-b3e1-972c60f47c8d">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="64bfd582-872a-4338-9d71-cfc40b705f09" key_ModelReferenceID="" referenceID="d739acb4-b988-4424-aa31-0d8580a6f3dd" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="9f74ff0b-d090-441a-80ca-08f704714e76">
                        <ParameterDependencyTermValue key_ParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" key_ParameterValueReferenceID="710e36c0-d9a9-43b2-b076-dd582392b405" key_ModelReferenceID="" referenceID="703927f9-3fe5-4247-94cc-8cb88c2c4184" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictTriggerTarget" label="RestrictTriggerTarget" comment="Don't have a template bind and triggers targetting the templated parent. TriggerTarget=TemplatedParent removes the background xaml attribute from the element." include="true" referenceID="352961a9-0578-4959-a6e8-8667390f40f1">
                <ParameterDependencyElement criteria="0" referenceID="d3e17a7e-f48c-42e4-a0a0-07bcfa15f5ac">
                    <ParameterDependencyCondition referenceID="9791b122-4181-4b68-bee2-1ffcc0857521">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" key_ParameterValueReferenceID="4b9882e6-6e04-48c0-ad3f-6c3841442ac9" key_ModelReferenceID="" referenceID="9413bad1-ed8f-44a7-8948-b90bea11ab90" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="40314fb7-54d2-4577-9dcf-f437b56045c3">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="64bfd582-872a-4338-9d71-cfc40b705f09" key_ModelReferenceID="" referenceID="6f1e7790-8414-4256-bcee-7deb6c46025c" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="031fcf6f-0176-4304-8626-7ec8c5d127a7">
                    <ParameterDependencyCondition referenceID="b4dcd8b9-0867-4aae-9bbb-d1a4155a3aef">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="560d14a2-a1f5-47b1-8b00-9d9678ba3145" key_ModelReferenceID="" referenceID="813a602f-c6b6-4f4a-a03f-f839e49a0a4b" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="bee46abc-89b4-47f1-a332-95308d8e5596">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="64bfd582-872a-4338-9d71-cfc40b705f09" key_ModelReferenceID="" referenceID="36eb3e67-f1a3-4a82-b308-66aa429648bf" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="8d872481-4819-4cf7-8266-d0f842d285cb">
                    <ParameterDependencyCondition referenceID="8bee3681-96a1-4982-a97f-1ce8616b6148">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6ce1ef66-8b2c-475c-942f-63b4b949683a" key_ModelReferenceID="" referenceID="2b509f0f-ef33-43f1-aa7f-35feaaec3e26" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="fd8cfe56-a29e-4108-9975-0defc96aef00">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="64bfd582-872a-4338-9d71-cfc40b705f09" key_ModelReferenceID="" referenceID="8688ab4a-1296-4a06-80f7-650439ca70db" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="14673335-7c02-44ee-8d28-867eec88e54b">
                    <ParameterDependencyCondition referenceID="04cb71a1-c254-4872-80cf-f2fa492ab8c3">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="85c133a5-b4ef-44fb-9894-718c3f391fb9" key_ModelReferenceID="" referenceID="bdc6c5a9-36b1-42bb-96bf-7b6ea07f6e42" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d1ec28b5-5146-49e8-9858-7fa17eaa6992">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="a3755416-7964-49a3-927f-97bef3adcb4d" key_ModelReferenceID="" referenceID="89099a40-7bee-41ed-b7ce-9e9760dcbc1c" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="97c2e1f0-f6f9-4a48-98c6-36621986d369">
                    <ParameterDependencyCondition referenceID="a7b06945-ac90-41d0-b200-f5ac5a3c7975">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="7800aefa-8e14-4ef6-86a1-1f478092d901" key_ModelReferenceID="" referenceID="94baa7d7-3eb6-4651-ae80-ec14b07f2aa7" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="d7ecfe65-eb04-43fb-9f50-a79cc5084b4c">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="a3755416-7964-49a3-927f-97bef3adcb4d" key_ModelReferenceID="" referenceID="d2cb5347-9a7a-4f3f-8be3-f3eb39e7331b" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="14b1fcb9-8866-43ce-8c5e-b552366a1048">
                    <ParameterDependencyCondition referenceID="2e091473-1e71-4eaa-8d13-2c7443b1edb7">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" key_ModelReferenceID="" referenceID="2b49abe0-70df-4718-8c38-ad8bdb87d60a" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="c97dfe55-33a0-4677-8327-12d0e6bcb6ae">
                        <ParameterDependencyTermValue key_ParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" key_ParameterValueReferenceID="a3755416-7964-49a3-927f-97bef3adcb4d" key_ModelReferenceID="" referenceID="d2f04c0e-51fd-49ac-9fdf-062ad766da31" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictHasPropertyTrigger" label="RestrictHasPropertyTrigger" comment="" include="true" referenceID="993b8aad-1056-4c35-b335-3c7c5245cf54">
                <ParameterDependencyElement criteria="0" referenceID="85895d29-6396-47de-b536-db9b9e1ef0ed">
                    <ParameterDependencyCondition referenceID="1b452d6f-72e4-4cd8-b538-54520498beb7">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="713d3516-64ad-4507-936c-1eac50ff9de4" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="4485c9ab-cd08-4167-ace1-99ef8de77398">
                        <ParameterDependencyTermValue key_ParameterReferenceID="afd2a1ae-cbf7-4242-8723-c52c89724542" key_ParameterValueReferenceID="4940ad92-ef33-458e-b432-4051259f2452" key_ModelReferenceID="" referenceID="0f501acd-3503-466b-a7f3-19e18263c257" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="856126e3-6b86-4182-9737-5e2bf532f651">
                    <ParameterDependencyCondition referenceID="415d6aa7-6da0-4f93-a4f5-b19f3da5c2ef">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="713d3516-64ad-4507-936c-1eac50ff9de4" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="4028f347-a434-4208-a2f3-d4ca365e54bc">
                        <ParameterDependencyTermValue key_ParameterReferenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1" key_ParameterValueReferenceID="2e0c2335-3fb7-4f8d-8a45-72ba32efcbbe" key_ModelReferenceID="" referenceID="01403e6b-7056-463c-ae6b-5f8558b7786f" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="1cccaf55-6749-4dfe-a03f-f3dd6252e591">
                    <ParameterDependencyCondition referenceID="ea7a4929-3a86-41f5-a4d9-c6e3d99a6dde">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="afd2a1ae-cbf7-4242-8723-c52c89724542" key_ParameterValueReferenceID="40633ef4-4b1e-40f5-acd4-05a349b45f44" key_ModelReferenceID="" referenceID="4f47cd53-b954-405d-a8e1-db4501ffc2c6" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="9f342515-f2c6-4908-aab9-29f40c7a4377">
                        <ParameterDependencyTermValue key_ParameterReferenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1" key_ParameterValueReferenceID="2e0c2335-3fb7-4f8d-8a45-72ba32efcbbe" key_ModelReferenceID="" referenceID="472a4839-4fcb-4d9e-9c18-28bfe8c3c81d" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictHasEventTrigger" label="RestrictHasEventTrigger" comment="" include="true" referenceID="9a554cc5-50d1-4d18-91a6-0406c8d55a74">
                <ParameterDependencyElement criteria="0" referenceID="7e0a643a-9111-4499-8627-f79a18fbb5a8">
                    <ParameterDependencyCondition referenceID="ac47478c-d052-4714-8f9d-56558340d993">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="af85680e-7ad5-4d5b-9cbf-105dbf1a3de5" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="e75c7060-05a0-4f0f-87dc-9406fc8a2dba">
                        <ParameterDependencyTermValue key_ParameterReferenceID="f3592b40-997b-479a-aa6c-643e28be9a19" key_ParameterValueReferenceID="d6e491f8-c4a2-4ce4-a669-c2582ec33f55" key_ModelReferenceID="" referenceID="a2af741d-aed9-4a6a-9cfe-5f72b83097cb" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
                <ParameterDependencyElement criteria="0" referenceID="b769aea3-b4e4-4053-aab6-4cba0332a5e0">
                    <ParameterDependencyCondition referenceID="f040e5eb-4e52-44c9-9204-6fba5e4c41aa">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" key_ParameterValueReferenceID="89083378-6bb9-4643-ad64-62944c67ef0c" key_ModelReferenceID="" referenceID="af85680e-7ad5-4d5b-9cbf-105dbf1a3de5" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="0c47032e-16da-477a-bcbe-1b69c5e99724">
                        <ParameterDependencyTermValue key_ParameterReferenceID="f3592b40-997b-479a-aa6c-643e28be9a19" key_ParameterValueReferenceID="ea844cef-0c7f-4c7b-8c1a-ed7cb8c22bd2" key_ModelReferenceID="" referenceID="54209376-3397-4779-801a-3af091e4bc01" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictTriggerSource" label="RestrictTriggerSource" comment="" include="true" referenceID="fd05dc24-1072-4b3e-8d29-6e213cfffd3c">
                <ParameterDependencyElement criteria="0" referenceID="c0230966-c11f-4467-8010-669642750bcf">
                    <ParameterDependencyCondition referenceID="897e2fad-6e5b-462d-be2f-97cea4f8f7c5">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" key_ParameterValueReferenceID="68e51a1a-b4d2-4d65-a011-dafb55c0ff6c" key_ModelReferenceID="" referenceID="e5ee7a86-78fe-4a55-b744-8a264acf07ff" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="480b3e16-37c2-4592-9bc4-3f799417f724">
                        <ParameterDependencyTermValue key_ParameterReferenceID="00a5fc9a-23eb-481e-b6dd-2c7d8ce7ab5c" key_ParameterValueReferenceID="d8a5ff3a-edc7-4466-b9f2-0cfdbcc166e5" key_ModelReferenceID="" referenceID="dffd3d2a-4246-4bfc-81f4-1c7cdf6b4fa2" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
            <ParameterDependencyGroup key_Name="RestrictKind" label="RestrictKind" comment="" include="true" referenceID="d4b08bae-abb0-4f19-9c7f-89666cb1dcdb">
                <ParameterDependencyElement criteria="0" referenceID="c40439dc-5d8b-4416-8d3a-4802e757c865">
                    <ParameterDependencyCondition referenceID="6faf6aee-778b-451c-b217-80054c6e2cc8">
                        <ParameterDependencyConditionValue key_ParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" key_ParameterValueReferenceID="6f256e3e-d023-446c-8b06-3a435140799e" key_ModelReferenceID="" referenceID="15ce9e5c-6da6-43aa-a9e4-78fc51ef896d" />
                    </ParameterDependencyCondition>
                    <ParameterDependencyTerm referenceID="e681350f-d229-4699-8af8-fbcc16fbdbe2">
                        <ParameterDependencyTermValue key_ParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" key_ParameterValueReferenceID="d4fe70a2-9abd-4aac-ba6e-86db6740c855" key_ModelReferenceID="" referenceID="4255024b-68d9-4825-8106-10448952b26e" />
                    </ParameterDependencyTerm>
                </ParameterDependencyElement>
            </ParameterDependencyGroup>
        </ParameterCombinationRules>
    </ModelParameters>
    <Actions referenceID="904eb823-9da6-4325-9ab6-582ca676b8c0">
        <Action key_Name="RunTest" label="RunTest" comment="" weight="1" disabled="false" defaultClusterReferenceID="" uiPersistentID="" uiPattern="" uiMethod="" automated="false" referenceID="35398109-adb6-4413-81c2-7d8aa0003e8b">
            <ActionParameters referenceID="86cdb6cd-0939-4694-8a28-11aaa8cb01e4">
                <ActionParameter key_ActionParameterReferenceID="f91b3683-ada7-481e-ba6e-c762448f6f65" referenceID="2c5b1de9-3a58-4773-8ee6-65e123937ceb" />
                <ActionParameter key_ActionParameterReferenceID="bebd9f74-6773-4972-82ae-24befbab6617" referenceID="2d8f58e4-48bd-4c47-b196-efd9b69f3003" />
                <ActionParameter key_ActionParameterReferenceID="85c3fba8-e6a1-4947-a416-f1431081b688" referenceID="e1917f74-b5f1-4918-bb0b-9a3be425abb5" />
                <ActionParameter key_ActionParameterReferenceID="29f29f8b-d0f4-49ae-86c2-35a2c4d4e142" referenceID="98d11427-46d6-4105-a5f8-e716bb8cdd83" />
                <ActionParameter key_ActionParameterReferenceID="5255e72a-30d8-4658-9b27-b4bcab3984da" referenceID="cb95e614-2b34-47e1-93db-c3908288df15" />
                <ActionParameter key_ActionParameterReferenceID="a0bf0f5a-5d0f-429d-b354-fc5cfab68b8f" referenceID="0dd246c8-eac3-4986-b4cf-b0539c340a27" />
                <ActionParameter key_ActionParameterReferenceID="6fdf0204-74aa-4710-ba88-58a0840a23c2" referenceID="753faacf-ff19-4c02-a88d-a393cba2ba24" />
                <ActionParameter key_ActionParameterReferenceID="803a4723-4db2-46aa-bcbc-5bdf3de8ef09" referenceID="4888e564-81b0-4f74-9486-866e7f777195" />
                <ActionParameter key_ActionParameterReferenceID="1c22008b-94c7-477e-9a31-8e0df96a90fe" referenceID="755d5f4b-8b50-49ac-9254-6473533f3494" />
                <ActionParameter key_ActionParameterReferenceID="afd2a1ae-cbf7-4242-8723-c52c89724542" referenceID="4f4d18bd-7c16-4d8b-98d4-f15590b2b7bf" />
                <ActionParameter key_ActionParameterReferenceID="ac9c0320-ea60-41f4-8de7-2190b653fbad" referenceID="06f1cd77-acf3-438b-97a8-469250985cd0" />
                <ActionParameter key_ActionParameterReferenceID="00a5fc9a-23eb-481e-b6dd-2c7d8ce7ab5c" referenceID="69d85596-8f6f-4a9f-a294-e876f1a25246" />
                <ActionParameter key_ActionParameterReferenceID="466dd4cf-7e95-4588-afe2-ad4eb9d1d0f1" referenceID="14763a04-06f8-4a62-8439-ddfa77ce4298" />
                <ActionParameter key_ActionParameterReferenceID="30b86b82-39c5-4ad1-82f9-a12fc0b9ab4c" referenceID="aa86322e-3307-4141-9277-4503f834ce0f" />
                <ActionParameter key_ActionParameterReferenceID="f3592b40-997b-479a-aa6c-643e28be9a19" referenceID="87d8c057-528a-49f5-bb95-b28df0ead811" />
                <ActionParameter key_ActionParameterReferenceID="cbe02e1b-a346-43b6-8423-bc5c1602f1bd" referenceID="a26e4890-8a75-42b4-9018-c06db527529b" />
                <ActionParameter key_ActionParameterReferenceID="e9036b04-fb98-4f0f-9e5b-a71c824f3106" referenceID="6f631636-77b7-460a-a31e-3721598311b7" />
                <ActionParameter key_ActionParameterReferenceID="0c0d3990-8281-4951-b7b4-8510e9d80851" referenceID="8bf97fb7-6542-40ed-8ceb-ee38310fef0c" />
                <ActionParameter key_ActionParameterReferenceID="d23e7747-ecb3-466b-8167-491ae2efc0f1" referenceID="616b442b-c33c-4788-b802-4186f2f8b50d" />
                <ActionParameter key_ActionParameterReferenceID="0dfd1ba8-90e7-417c-a1d3-82481389cf59" referenceID="6ba6c990-c200-468b-92d7-003d55fece90" />
            </ActionParameters>
        </Action>
    </Actions>
    <TransitionRules referenceID="282f6d76-9e3b-4fee-b7c1-ff43c45d9774" />
    <EntryPoints seed="1" referenceID="f1a014be-923e-48a2-b22c-52667d91d9fb" />
    <AssertionRules referenceID="040937e9-6ca1-4870-ad4f-59f754a1e87c" />
    <ModelInfo overview="" constructionInfo="" referenceID="5d281004-77d0-42ab-b041-c0af86f8d28b" />
    <ModelReferences referenceID="c119949c-bb8c-4c6d-aa18-174026cac5c3" />
    <ModelLinkRules referenceID="5bf0d0ab-b98a-43f0-ab94-a5be23af4177" />
    <TestQueries referenceID="68cdf4fc-7761-4ce2-a4d8-17c493843769">
        <TestQuery key_Name="GeneratePairwise" label="GeneratePairwise" testOutputFilePathName="&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;&#xD;&#xA;&lt;StandAloneLocation xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;&#xD;&#xA;  &lt;ModelPathName&gt;F:\nt\testsrc\windowstestdata\REDIST\client\wcptests\core\bvt\propertyengine\Template\Template.mbt&lt;/ModelPathName&gt;&#xD;&#xA;  &lt;TestCaseFileName&gt;F:\nt\testsrc\windowstestdata\REDIST\client\wcptests\core\bvt\propertyengine\Template\TemplateModel_Pairwise.xtc&lt;/TestCaseFileName&gt;&#xD;&#xA;&lt;/StandAloneLocation&gt;" comment="" isStateless="true" referenceID="a5f941a3-c5b2-4a7f-abaf-e4adc7cd00a0" maxTestCases="100" applyFilters="true" testOutputLocationType="Microsoft.Ite.StandAloneInterface.StandAloneLocation,Microsoft.Ite.StandAloneTcmInterface, Version=2.1.2005.0, Culture=neutral, PublicKeyToken=null~" externalDataXml="" externalDataType="" disabled="false">
            <TestPattern seed="1" referenceID="50b0ae70-d7a6-4616-a5dd-1eb15fcc5b87">
                <TestPatternElement testPatternElementType="0" modelReferenceID="642091e3-c596-41d9-8816-50320d0ffff1" referenceID="143c9c79-fb01-4540-bc03-09fc928d6334">
                    <ActionTestPatternElement actionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" referenceID="91c6ea36-e079-4ec4-81a7-412e8fbba00d" />
                </TestPatternElement>
                <TestPatternElement testPatternElementType="3" modelReferenceID="642091e3-c596-41d9-8816-50320d0ffff1" referenceID="112fcc61-c4cd-4f0d-bcac-63441f46c862">
                    <GlobalFilterTestPatternElement lowWeight="0" highWeight="0" validFilter="1" automatedFilter="0" bucketFilter="" maxCombinationType="0" maxCombinationValue="0" mileStone="" generationType="0" referenceID="bf853c0f-f35d-436c-97f6-89934ecfbc56" />
                </TestPatternElement>
            </TestPattern>
        </TestQuery>
        <TestQuery key_Name="GenerateThreeTuple" label="GenerateThreeTuple" testOutputFilePathName="&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;&#xD;&#xA;&lt;StandAloneLocation xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;&#xD;&#xA;  &lt;ModelPathName&gt;E:\src\testsrc\windowstestdata\REDIST\client\wcptests\core\bvt\propertyengine\Template\Template.mbt&lt;/ModelPathName&gt;&#xD;&#xA;  &lt;TestCaseFileName&gt;e:\src\testsrc\windowstestdata\redist\client\wcptests\core\bvt\propertyengine\Template\TemplateModel_ThreeTuple.xtc&lt;/TestCaseFileName&gt;&#xD;&#xA;&lt;/StandAloneLocation&gt;" comment="" isStateless="true" referenceID="5c905116-8bb4-47ca-b010-6a77406fab27" maxTestCases="600" applyFilters="true" testOutputLocationType="Microsoft.Ite.StandAloneInterface.StandAloneLocation,Microsoft.Ite.StandAloneTcmInterface, Version=2.1.2005.0, Culture=neutral, PublicKeyToken=null~" externalDataXml="" externalDataType="" disabled="false">
            <TestPattern seed="1" referenceID="624d0b98-717f-43bf-9fda-823dfca156e7">
                <TestPatternElement testPatternElementType="0" modelReferenceID="642091e3-c596-41d9-8816-50320d0ffff1" referenceID="584a5a41-80c7-46f0-9287-88d2b7a63067">
                    <ActionTestPatternElement actionReferenceID="35398109-adb6-4413-81c2-7d8aa0003e8b" referenceID="abfd9190-e5b5-4b25-8fc7-a7669b8b03c5" />
                </TestPatternElement>
            </TestPattern>
        </TestQuery>
    </TestQueries>
    <TestFilters referenceID="7b7d67f6-169a-454a-b8ac-5d86be509044" />
    <CustomData referenceID="357da1fa-17c3-4c31-a7f1-ba33eeeb06d7" />
</IteModel>

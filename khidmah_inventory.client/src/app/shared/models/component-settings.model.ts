/**
 * Comprehensive Component Settings Models
 * All UI components can be customized through these settings
 */

export interface BaseComponentSettings {
  id?: string;
  variant?: string;
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | string;
  style?: 'material' | 'bootstrap' | 'custom';
  customClass?: string;
  disabled?: boolean;
  visible?: boolean;
}

// Button Settings
export interface ButtonSettings extends BaseComponentSettings {
  variant: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark';
  type: 'button' | 'submit' | 'reset';
  loading?: boolean;
  icon?: string;
  iconPosition?: 'left' | 'right';
  iconLibrary?: 'fa' | 'material';
  fullWidth?: boolean;
  outlined?: boolean;
  raised?: boolean;
  tooltip?: string;
  borderRadius?: string;
  padding?: string;
  fontSize?: string;
  fontWeight?: string;
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  hoverBackgroundColor?: string;
  hoverTextColor?: string;
  activeBackgroundColor?: string;
}

// Input Settings
export interface InputSettings extends BaseComponentSettings {
  label?: string;
  placeholder?: string;
  type: 'text' | 'email' | 'password' | 'tel' | 'number' | 'url' | 'search' | 'date' | 'time' | 'datetime-local';
  required?: boolean;
  readonly?: boolean;
  icon?: string;
  iconPosition?: 'left' | 'right';
  iconLibrary?: 'fa' | 'material';
  error?: string;
  hint?: string;
  autocomplete?: string;
  min?: number | string;
  max?: number | string;
  step?: number;
  pattern?: string;
  maxLength?: number;
  minLength?: number;
  borderColor?: string;
  focusBorderColor?: string;
  errorBorderColor?: string;
  backgroundColor?: string;
  textColor?: string;
}

// Card Settings
export interface CardSettings extends BaseComponentSettings {
  variant: 'default' | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
  elevation?: 0 | 1 | 2 | 3 | 4 | 5;
  outlined?: boolean;
  header?: string;
  footer?: string;
  hoverable?: boolean;
  clickable?: boolean;
  backgroundColor?: string;
  borderColor?: string;
  borderRadius?: string;
  padding?: string;
  shadow?: string;
}

// Table Settings
export interface TableSettings extends BaseComponentSettings {
  striped?: boolean;
  bordered?: boolean;
  hoverable?: boolean;
  responsive?: boolean;
  pagination?: boolean;
  sorting?: boolean;
  filtering?: boolean;
  pageSize?: number;
  pageSizeOptions?: number[];
  showCheckbox?: boolean;
  showActions?: boolean;
  stickyHeader?: boolean;
  stickyFooter?: boolean;
  headerBackgroundColor?: string;
  headerTextColor?: string;
  rowBackgroundColor?: string;
  rowHoverColor?: string;
  borderColor?: string;
  fontSize?: string;
  rowHeight?: string;
}

// Modal Settings
export interface ModalSettings extends BaseComponentSettings {
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'fullscreen';
  backdrop?: boolean | 'static';
  keyboard?: boolean;
  centered?: boolean;
  scrollable?: boolean;
  closeOnOutsideClick?: boolean;
  showHeader?: boolean;
  showFooter?: boolean;
  headerTitle?: string;
  footerButtons?: ButtonSettings[];
  backgroundColor?: string;
  backdropColor?: string;
  borderRadius?: string;
  maxWidth?: string;
  maxHeight?: string;
}

// Drawer Settings
export interface DrawerSettings extends BaseComponentSettings {
  position?: 'left' | 'right' | 'top' | 'bottom';
  width?: string;
  height?: string;
  backdrop?: boolean;
  closeOnOutsideClick?: boolean;
  showHeader?: boolean;
  headerTitle?: string;
  backgroundColor?: string;
  backdropColor?: string;
  borderRadius?: string;
}

// List Settings
export interface ListSettings extends BaseComponentSettings {
  variant?: 'default' | 'bordered' | 'striped' | 'hoverable';
  dense?: boolean;
  selectable?: boolean;
  multiSelect?: boolean;
  showIcons?: boolean;
  showAvatars?: boolean;
  showActions?: boolean;
  itemPadding?: string;
  itemBorderRadius?: string;
  itemHoverColor?: string;
  itemSelectedColor?: string;
  backgroundColor?: string;
  borderColor?: string;
}

// Menu Settings
export interface MenuSettings extends BaseComponentSettings {
  position?: 'below' | 'above' | 'before' | 'after';
  trigger?: 'click' | 'hover';
  showIcons?: boolean;
  showDividers?: boolean;
  dense?: boolean;
  maxHeight?: string;
  backgroundColor?: string;
  textColor?: string;
  hoverColor?: string;
  borderRadius?: string;
  shadow?: string;
}

// Select Settings
export interface SelectSettings extends BaseComponentSettings {
  label?: string;
  placeholder?: string;
  multiple?: boolean;
  searchable?: boolean;
  clearable?: boolean;
  required?: boolean;
  options?: SelectOption[];
  error?: string;
  hint?: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
}

export interface SelectOption {
  value: any;
  label: string;
  disabled?: boolean;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  group?: string;
}

// Textarea Settings
export interface TextareaSettings extends BaseComponentSettings {
  label?: string;
  placeholder?: string;
  rows?: number;
  cols?: number;
  required?: boolean;
  readonly?: boolean;
  resize?: 'none' | 'both' | 'horizontal' | 'vertical';
  maxLength?: number;
  minLength?: number;
  error?: string;
  hint?: string;
}

// Checkbox Settings
export interface CheckboxSettings extends BaseComponentSettings {
  label?: string;
  checked?: boolean;
  indeterminate?: boolean;
  required?: boolean;
  color?: string;
  size?: 'sm' | 'md' | 'lg';
}

// Radio Settings
export interface RadioSettings extends BaseComponentSettings {
  label?: string;
  value?: any;
  checked?: boolean;
  required?: boolean;
  color?: string;
  size?: 'sm' | 'md' | 'lg';
}

// Switch Settings
export interface SwitchSettings extends BaseComponentSettings {
  label?: string;
  checked?: boolean;
  required?: boolean;
  color?: string;
  size?: 'sm' | 'md' | 'lg';
}

// DatePicker Settings
export interface DatePickerSettings extends BaseComponentSettings {
  label?: string;
  placeholder?: string;
  required?: boolean;
  minDate?: Date | string;
  maxDate?: Date | string;
  format?: string;
  showTime?: boolean;
  error?: string;
  hint?: string;
}

// FileUpload Settings
export interface FileUploadSettings extends BaseComponentSettings {
  label?: string;
  accept?: string;
  multiple?: boolean;
  maxSize?: number;
  maxFiles?: number;
  required?: boolean;
  dragDrop?: boolean;
  preview?: boolean;
  error?: string;
  hint?: string;
}

// Badge Settings
export interface BadgeSettings extends BaseComponentSettings {
  variant: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark';
  content: string | number;
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
  dot?: boolean;
  max?: number;
  backgroundColor?: string;
  textColor?: string;
  borderRadius?: string;
}

// Alert Settings
export interface AlertSettings extends BaseComponentSettings {
  variant: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
  dismissible?: boolean;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  title?: string;
  message?: string;
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  borderRadius?: string;
}

// Progress Settings
export interface ProgressSettings extends BaseComponentSettings {
  value: number;
  max?: number;
  variant?: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
  striped?: boolean;
  animated?: boolean;
  showLabel?: boolean;
  height?: string;
  backgroundColor?: string;
  progressColor?: string;
  borderRadius?: string;
}

// Tabs Settings
export interface TabsSettings extends BaseComponentSettings {
  tabs: TabItem[];
  activeTab?: number;
  position?: 'top' | 'bottom' | 'left' | 'right';
  variant?: 'default' | 'pills' | 'underline';
  fullWidth?: boolean;
  backgroundColor?: string;
  activeColor?: string;
  inactiveColor?: string;
  borderColor?: string;
}

export interface TabItem {
  label: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  disabled?: boolean;
  badge?: string | number;
}

// Accordion Settings
export interface AccordionSettings extends BaseComponentSettings {
  items: AccordionItem[];
  allowMultiple?: boolean;
  variant?: 'default' | 'bordered' | 'outlined';
  backgroundColor?: string;
  headerBackgroundColor?: string;
  borderColor?: string;
  borderRadius?: string;
}

export interface AccordionItem {
  title: string;
  content: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  disabled?: boolean;
  expanded?: boolean;
}

// Tooltip Settings
export interface TooltipSettings extends BaseComponentSettings {
  content: string;
  position?: 'top' | 'bottom' | 'left' | 'right';
  trigger?: 'hover' | 'click' | 'focus';
  delay?: number;
  backgroundColor?: string;
  textColor?: string;
  borderRadius?: string;
  maxWidth?: string;
}

// Popover Settings
export interface PopoverSettings extends BaseComponentSettings {
  title?: string;
  content: string;
  position?: 'top' | 'bottom' | 'left' | 'right';
  trigger?: 'click' | 'hover' | 'focus';
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  borderRadius?: string;
  shadow?: string;
}

// Dropdown Settings
export interface DropdownSettings extends BaseComponentSettings {
  items: DropdownItem[];
  trigger?: 'click' | 'hover';
  position?: 'bottom-left' | 'bottom-right' | 'top-left' | 'top-right';
  showIcons?: boolean;
  showDividers?: boolean;
  backgroundColor?: string;
  textColor?: string;
  hoverColor?: string;
  borderRadius?: string;
  shadow?: string;
}

export interface DropdownItem {
  label: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  action?: () => void;
  disabled?: boolean;
  divider?: boolean;
  badge?: string | number;
}

// Pagination Settings
export interface PaginationSettings extends BaseComponentSettings {
  currentPage: number;
  totalPages: number;
  pageSize?: number;
  showFirstLast?: boolean;
  showPrevNext?: boolean;
  showPageNumbers?: boolean;
  maxVisiblePages?: number;
  variant?: 'default' | 'outlined' | 'rounded';
  size?: 'sm' | 'md' | 'lg';
  backgroundColor?: string;
  activeColor?: string;
  textColor?: string;
  borderColor?: string;
  borderRadius?: string;
}

// Breadcrumb Settings
export interface BreadcrumbSettings extends BaseComponentSettings {
  items: BreadcrumbItem[];
  separator?: string | 'slash' | 'arrow' | 'dot';
  showIcons?: boolean;
  backgroundColor?: string;
  textColor?: string;
  activeColor?: string;
  separatorColor?: string;
}

export interface BreadcrumbItem {
  label: string;
  route?: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
}

// Stepper Settings
export interface StepperSettings extends BaseComponentSettings {
  steps: StepperStep[];
  currentStep?: number;
  orientation?: 'horizontal' | 'vertical';
  linear?: boolean;
  variant?: 'default' | 'outlined' | 'filled';
  activeColor?: string;
  inactiveColor?: string;
  completedColor?: string;
  errorColor?: string;
}

export interface StepperStep {
  label: string;
  description?: string;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  completed?: boolean;
  error?: boolean;
  disabled?: boolean;
}

// Chip Settings
export interface ChipSettings extends BaseComponentSettings {
  label: string;
  variant?: 'default' | 'outlined' | 'filled';
  color?: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
  removable?: boolean;
  icon?: string;
  iconLibrary?: 'fa' | 'material';
  avatar?: string;
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  borderRadius?: string;
}

// Avatar Settings
export interface AvatarSettings extends BaseComponentSettings {
  src?: string;
  alt?: string;
  text?: string;
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl';
  shape?: 'circle' | 'square' | 'rounded';
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  borderWidth?: string;
}

// Skeleton Settings
export interface SkeletonSettings extends BaseComponentSettings {
  variant?: 'text' | 'circular' | 'rectangular';
  width?: string;
  height?: string;
  animation?: 'pulse' | 'wave' | 'none';
  backgroundColor?: string;
  highlightColor?: string;
}

// Divider Settings
export interface DividerSettings extends BaseComponentSettings {
  orientation?: 'horizontal' | 'vertical';
  variant?: 'solid' | 'dashed' | 'dotted';
  color?: string;
  thickness?: string;
  spacing?: string;
}

// Spacer Settings
export interface SpacerSettings {
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl';
  height?: string;
  width?: string;
}

// Grid Settings
export interface GridSettings {
  columns?: number;
  gap?: string;
  responsive?: boolean;
  breakpoints?: {
    xs?: number;
    sm?: number;
    md?: number;
    lg?: number;
    xl?: number;
  };
}

// Container Settings
export interface ContainerSettings {
  maxWidth?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'full';
  fluid?: boolean;
  padding?: string;
  backgroundColor?: string;
}

// Global Settings
export interface GlobalComponentSettings {
  buttons: { [key: string]: ButtonSettings };
  inputs: { [key: string]: InputSettings };
  cards: { [key: string]: CardSettings };
  tables: { [key: string]: TableSettings };
  modals: { [key: string]: ModalSettings };
  drawers: { [key: string]: DrawerSettings };
  lists: { [key: string]: ListSettings };
  menus: { [key: string]: MenuSettings };
  selects: { [key: string]: SelectSettings };
  textareas: { [key: string]: TextareaSettings };
  checkboxes: { [key: string]: CheckboxSettings };
  radios: { [key: string]: RadioSettings };
  switches: { [key: string]: SwitchSettings };
  datePickers: { [key: string]: DatePickerSettings };
  fileUploads: { [key: string]: FileUploadSettings };
  badges: { [key: string]: BadgeSettings };
  alerts: { [key: string]: AlertSettings };
  progress: { [key: string]: ProgressSettings };
  tabs: { [key: string]: TabsSettings };
  accordions: { [key: string]: AccordionSettings };
  tooltips: { [key: string]: TooltipSettings };
  popovers: { [key: string]: PopoverSettings };
  dropdowns: { [key: string]: DropdownSettings };
  pagination: { [key: string]: PaginationSettings };
  breadcrumbs: { [key: string]: BreadcrumbSettings };
  steppers: { [key: string]: StepperSettings };
  chips: { [key: string]: ChipSettings };
  avatars: { [key: string]: AvatarSettings };
  skeletons: { [key: string]: SkeletonSettings };
  dividers: { [key: string]: DividerSettings };
  spacers: { [key: string]: SpacerSettings };
  grids: { [key: string]: GridSettings };
  containers: { [key: string]: ContainerSettings };
}


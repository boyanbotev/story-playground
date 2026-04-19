import { styled } from '@mui/material/styles';
import Button from '@mui/material/Button';
import type { ButtonProps } from '@mui/material/Button';

const MyStyledButton = styled(Button)({
    textTransform: 'initial',
    margin: '0.5rem',
    padding: '0.5rem',
    border: '1px solid black',
    borderRadius: '5px',
});

interface StyledButtonProps extends ButtonProps {
    to?: string;
}

export const StyledButton = ({ children, ...props }: StyledButtonProps) => {
  return (
    <MyStyledButton {...props}>
      {children}
    </MyStyledButton>
  );
};
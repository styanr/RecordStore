import { Box } from '@chakra-ui/react';

import { getContrastColor } from '../utils/getContrastColor';

const CustomBadge = ({ color, children, ...props }) => (
  <Box
    bg={color}
    color={getContrastColor(color)}
    px={2}
    py={1}
    borderRadius='md'
    fontWeight='bold'
    fontSize='sm'
    textTransform='uppercase'
    w='fit-content'
    {...props}
  >
    {children}
  </Box>
);

export default CustomBadge;
